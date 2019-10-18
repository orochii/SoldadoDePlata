using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour {
    [SerializeField] private Animator charAnim;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float godSpeedMult = 20f;
    [SerializeField] private float godSpeedMax = 120f;
    [SerializeField] private float maxVerticalSpeed = 20f;
    [SerializeField] private float attackCooldown = 0.8f;

    internal void SetBusy(bool v) {
        selfChar.Busy = v;
    }

    [SerializeField] private float grabTossCooldown = 1.2f;
    [SerializeField] private float angleLerp = 0.9f;
    [SerializeField] private float jumpSpeed = 6.5f;
    [SerializeField] private float endJumpSpeed = 4.5f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float maxSlope = 60f;
    [SerializeField] private AttackController animController;
    [SerializeField] private ParticleSystem godSpeedEffect;
    [SerializeField] private LayerMask storableGeometry;

    private bool jumpPressed;
    private float attackTimer;
    private float jumpTimer;
    private float coyoteTimer;
    private float sqrGodspeedMax;
    private Rigidbody rbody;
    private CapsuleCollider capsule;
    private PlayerGrab grab;
    private Character selfChar;
    private CameraControl cameraControl;
    private Vector3 lastGroundedPosition;
    // Animation hashes
    private int hashFMoveSpeed;
    private int hashFAnimSpeed;
    private int hashTAttack;
    private int hashTGrab;
    private int hashBJumping;
    private int hashBGodspeed;
    //
    private bool godSpeed, jump, jumpStart;
    private float horz, vert;
    private Vector3 movement;
    //
    private Coroutine fallCoroutine;

    public void SetRespawn() {
        GameManager.Instance.respawnPosition = transform.position;
    }

    void Awake() {
        rbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        grab = GetComponentInChildren<PlayerGrab>();
        hashFMoveSpeed = Animator.StringToHash("move_speed");
        hashTAttack = Animator.StringToHash("attack");
        hashTGrab = Animator.StringToHash("grab");
        hashBJumping = Animator.StringToHash("jumping");
        hashBGodspeed = Animator.StringToHash("godspeed");
        hashFAnimSpeed = Animator.StringToHash("animation_speed");
        sqrGodspeedMax = godSpeedMax * godSpeedMax;
    }
    private void Start() {
        selfChar = GetComponent<Character>();
        cameraControl = FindObjectOfType<CameraControl>();
    }

    private void Update() {
        if (selfChar.Dead || selfChar.Busy) {
            //TODO: Death animation
            horz = 0;
            vert = 0;
            godSpeed = false;
            jump = false;
            jumpStart = false;
            movement = Vector3.zero;
            return;
        }
        horz = GameEvent.Waiting ? 0 : Input.GetAxisRaw("Horizontal");
        vert = GameEvent.Waiting ? 0 : Input.GetAxisRaw("Vertical");
        godSpeed = !GameEvent.Waiting && GameManager.GetSwitch(0) && Input.GetAxis("Fire3") != 0; //Input.GetButton("Fire3");
        bool attack = !godSpeed && !GameEvent.Waiting && Input.GetButtonDown("Fire1");
        bool grabThrow = !godSpeed && !GameEvent.Waiting && Input.GetButtonUp("Fire2");
        jump = !GameEvent.Waiting && Input.GetButton("Jump");
        jumpStart = (jumpStart) || (!GameEvent.Waiting && Input.GetButtonDown("Jump"));
        // Move player
        movement = new Vector3(horz, 0, vert).normalized;
        movement = Quaternion.Euler(0, cameraControl.RotationY, 0) * movement * moveSpeed;
        // EXECUTE ATTACK
        if (attackTimer < Time.time) {
            if (attack && !grab.IsGrabbing) {
                Attack();
                return;
            }
            // GRAB/THROW OBJECTS
            if (grabThrow) {
                GrabThrow();
                return;
            }
        } else {

        }
    }

    void FixedUpdate() {
        //if (selfChar.Dead || selfChar.Busy) return;
        // AUTORECOVER
        if (GameManager.GetSwitch(1)) {
            if (!selfChar.Dead) selfChar.HP += 1f * Time.deltaTime;
        }
        // GODSPEED
        if (godSpeed) {
            Vector3 m = cameraControl.Rotation * new Vector3(horz, 0, vert);
            // Rotate player towards movement
            if (m.sqrMagnitude > 0) {
                Quaternion flyDir = Quaternion.LookRotation(m, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, flyDir, angleLerp);
            }
            charAnim.SetBool(hashBGodspeed, true);
            godSpeedEffect.Play();
            // Check slope in front
            Vector3 gsMove = CheckFlythrough(m, godSpeedMult * moveSpeed); //m * godSpeedMult; //
            rbody.AddForce(gsMove);
            if (rbody.velocity.sqrMagnitude > sqrGodspeedMax) {
                Vector3 vel = rbody.velocity.normalized;
                rbody.velocity = vel * godSpeedMax;
            }
            return;
        } else {
            charAnim.SetBool(hashBGodspeed, false);
            godSpeedEffect.Stop();
        }
        
        // Rotate player towards movement
        if (movement.sqrMagnitude > 0) {
            float angle = Vector3.SignedAngle(Vector3.forward, movement, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0), angleLerp);
        }
        // Check slope in front
        movement = CheckFront(movement);
        // Fall speed
        float zVelocity = Mathf.Clamp(rbody.velocity.y, -maxVerticalSpeed, maxVerticalSpeed);
        // JUMPING
        // If it finds something below, then it's grounded (probably not gonna work, but let's assume it does for now :^D)
        // Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f)
        RaycastHit hit;
        bool cast = Physics.BoxCast(transform.position + Vector3.up * 0.1f, new Vector3(.15f, .025f, .15f), Vector3.down, out hit, transform.rotation, 0.1f);
        if (cast) coyoteTimer = Time.time + coyoteTime;
        if (zVelocity <= 0.005f && (coyoteTimer > Time.time)) {
            if (cast && IsInLayerMask(hit.collider.gameObject.layer)) {
                lastGroundedPosition = transform.position;
            }
            if (jumpStart) {
                zVelocity = jumpSpeed;
                charAnim.SetBool(hashBJumping, true);
                jumpPressed = true;
                jumpTimer = Time.time + jumpCooldown;
            } else {
                charAnim.SetBool(hashBJumping, false);
            }
        } else if (!cast) {
            charAnim.SetBool(hashBJumping, true);
        }
        jumpStart = false;
        if (jumpPressed) {
            if (!jump || jumpTimer < Time.time) {
                jumpPressed = false;
                if (!jump && zVelocity > endJumpSpeed) zVelocity = endJumpSpeed;
            } else zVelocity = jumpSpeed;
        }
        movement.y = zVelocity;
        // Submit velocity change
        rbody.velocity = movement;
        // Animate player
        float v = (horz != 0 || vert != 0) ? 1 : 0;
        charAnim.SetFloat(hashFMoveSpeed, v);
        charAnim.SetFloat(hashFAnimSpeed, 1 + v * 3);
    }

    private bool IsInLayerMask(int layer) {
        return (((storableGeometry >> layer) & 1) == 1);
    }

    private Vector3 CheckFlythrough(Vector3 movement, float distance) {
        // Spherecast to all looking for something in front
        Vector3 origin = transform.position + capsule.center;
        RaycastHit[] cHits = Physics.SphereCastAll(origin, capsule.radius, movement, rbody.velocity.magnitude);
        foreach (RaycastHit cHit in cHits) {
            if (!cHit.collider.isTrigger && cHit.collider.attachedRigidbody != rbody) {
                return movement.normalized * cHit.distance;
            }
        }
        return movement.normalized * distance;
    }

    private Vector3 CheckFront(Vector3 movement) {
        // Spherecast to all looking for something in front
        Vector3 origin = transform.position + capsule.center;
        RaycastHit[] cHits = Physics.SphereCastAll(origin, capsule.radius, movement, 0.33f);
        foreach (RaycastHit cHit in cHits) {
            if (!cHit.collider.isTrigger && cHit.collider.attachedRigidbody != rbody) {
                Vector3 n = cHit.normal;
                n.y = 0;
                movement += n * moveSpeed;
                return movement;
            }
        }
        return movement;
    }
    
    private void Attack() {
        charAnim.SetTrigger(hashTAttack);
        attackTimer = Time.time + attackCooldown;
    }

    private void GrabThrow() {
        charAnim.SetTrigger(hashTGrab);
        attackTimer = Time.time + grabTossCooldown;
    }

    public void Respawn() {
        if (fallCoroutine != null) StopCoroutine(fallCoroutine);
        fallCoroutine = StartCoroutine(DoRespawn());
    }

    private IEnumerator DoRespawn() {
        if (cameraControl != null) cameraControl.gameObject.SetActive(false);
        if (selfChar != null) selfChar.Busy = true;
        yield return new WaitForSeconds(5f);
        GoToRespawn();
        selfChar.Revive();
        if (cameraControl != null) cameraControl.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);
        if (selfChar != null) selfChar.Busy = false;
    }

    public void GoToRespawn() {
        rbody.velocity = Vector3.zero;
        transform.position = GameManager.Instance.respawnPosition;
        animController.Flicker(0.25f, 8);
    }

    public void OnFall() {
        if (fallCoroutine != null) StopCoroutine(fallCoroutine);
        fallCoroutine = StartCoroutine(DoFallRecovery());
    }

    private IEnumerator DoFallRecovery() {
        if (cameraControl != null) cameraControl.gameObject.SetActive(false);
        if (selfChar != null) selfChar.Busy = true;
        yield return new WaitForSeconds(2f);
        GoToLastGround();
        if (cameraControl != null) cameraControl.gameObject.SetActive(true);
        if (selfChar != null) selfChar.Busy = false;
    }

    public void GoToLastGround() {
        rbody.velocity = Vector3.zero;
        transform.position = lastGroundedPosition;
        animController.Flicker(0.25f, 8);
    }
}