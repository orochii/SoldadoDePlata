using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private AttackController animController;

    private bool jumpPressed;
    private float attackTimer;
    private float jumpTimer;
    private Rigidbody rbody;
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
        float horz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        bool attack = Input.GetButtonDown("Fire1");
        bool grabThrow = Input.GetButtonUp("Fire2");
        bool jump = Input.GetButton("Jump");
        bool jumpStart = Input.GetButtonDown("Jump");
        // Move player
        Vector3 movement = new Vector3(horz, 0, vert).normalized;
        movement = Quaternion.Euler(0, cameraControl.RotationY, 0) * movement * moveSpeed;
        // Rotate player towards movement
        if (movement.sqrMagnitude > 0) {
            float angle = Vector3.SignedAngle(Vector3.forward, movement, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0), angleLerp);
        }
        // Fall speed
        float zVelocity = Mathf.Clamp(rbody.velocity.y, -maxVerticalSpeed, maxVerticalSpeed);
        // Jumping
        RaycastHit hit;
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
            }
            else zVelocity = jumpSpeed;
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
            if (attack) {
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
        transform.position = lastGroundedPosition;
        animController.Flicker(0.25f, 8);
    }
}