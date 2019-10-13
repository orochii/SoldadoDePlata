using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour {
    [SerializeField] private Animator charAnim;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxVerticalSpeed = 20f;
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private float grabTossCooldown = 1.2f;
    [SerializeField] private float angleLerp = 0.9f;
    [SerializeField] private float jumpSpeed = 6.5f;
    [SerializeField] private float endJumpSpeed = 4.5f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float maxSlope = 60f;
    [SerializeField] private AttackController animController;
    
    private bool jumpPressed;
    private float attackTimer;
    private float jumpTimer;
    private Rigidbody rbody;
    private CapsuleCollider capsule;
    private PlayerGrab grab;
    private CameraControl cameraControl;
    private Vector3 lastGroundedPosition;
    // Animation hashes
    private int hashFMoveSpeed;
    private int hashFAnimSpeed;
    private int hashTAttack;
    private int hashTGrab;
    private int hashBJumping;
    //
    private Coroutine fallCoroutine;

    void Awake() {
        rbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        grab = GetComponentInChildren<PlayerGrab>();
        hashFMoveSpeed = Animator.StringToHash("move_speed");
        hashTAttack = Animator.StringToHash("attack");
        hashTGrab = Animator.StringToHash("grab");
        hashBJumping = Animator.StringToHash("jumping");
        hashFAnimSpeed = Animator.StringToHash("animation_speed");
    }
    private void Start() {
        cameraControl = FindObjectOfType<CameraControl>();
    }

    void Update() {
        float horz = GameEvent.Waiting ? 0 : Input.GetAxisRaw("Horizontal");
        float vert = GameEvent.Waiting ? 0 : Input.GetAxisRaw("Vertical");
        bool attack = !GameEvent.Waiting && Input.GetButtonDown("Fire1");
        bool grabThrow = !GameEvent.Waiting && Input.GetButtonUp("Fire2");
        bool jump = !GameEvent.Waiting && Input.GetButton("Jump");
        bool jumpStart = !GameEvent.Waiting && Input.GetButtonDown("Jump");
        // Move player
        Vector3 movement = new Vector3(horz, 0, vert).normalized;
        movement = Quaternion.Euler(0, cameraControl.RotationY, 0) * movement * moveSpeed;
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
        bool cast = Physics.BoxCast(transform.position + Vector3.up * 0.1f, new Vector3(.15f, .025f, .15f), Vector3.down, transform.rotation, 0.1f);
        if (zVelocity <= 0.005f && cast) {
            lastGroundedPosition = transform.position;
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

    public void OnFall() {
        if (fallCoroutine != null) StopCoroutine(fallCoroutine);
        fallCoroutine = StartCoroutine(DoFallRecovery());
    }
    private IEnumerator DoFallRecovery() {
        CameraControl cc = GameObject.FindObjectOfType<CameraControl>();
        Character selfChar = GetComponent<Character>();
        if (cc != null) cc.gameObject.SetActive(false);
        if (selfChar != null) selfChar.Busy = true;
        yield return new WaitForSeconds(2f);
        GoToLastGround();
        if (cc != null) cc.gameObject.SetActive(true);
        if (selfChar != null) selfChar.Busy = false;
    }

    public void GoToLastGround() {
        rbody.velocity = Vector3.zero;
        transform.position = lastGroundedPosition;
        animController.Flicker(0.25f, 8);
    }
}