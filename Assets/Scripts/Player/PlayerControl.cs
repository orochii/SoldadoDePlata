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
    [SerializeField] private float angleLerp = 0.9f;
    [SerializeField] private float jumpSpeed = 6.5f;

    private float attackTimer;
    private Rigidbody rbody;
    private CameraControl cameraControl;
    // Animation hashes
    private int hashFMoveSpeed;
    private int hashTAttack;
    private int hashBJumping;

    void Awake() {
        rbody = GetComponent<Rigidbody>();
        hashFMoveSpeed = Animator.StringToHash("move_speed");
        hashTAttack = Animator.StringToHash("attack");
        hashBJumping = Animator.StringToHash("jumping");
    }
    private void Start() {
        cameraControl = FindObjectOfType<CameraControl>();
    }

    void Update() {
        float horz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        bool attack = Input.GetButtonDown("Fire1");
        bool jump = Input.GetButtonUp("Jump");
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
        Debug.Log(cast);
        if (zVelocity <= 0.005f && cast) {
            if (jump) {
                zVelocity = jumpSpeed;
                charAnim.SetBool(hashBJumping, true);
            } else {
                charAnim.SetBool(hashBJumping, false);
            }
        }
        movement.y = zVelocity;
        // Submit velocity change
        rbody.velocity = movement;
        // Animate player
        float v = (horz != 0 || vert != 0) ? 1 : 0;
        charAnim.SetFloat(hashFMoveSpeed, v);
        // EXECUTE ATTACK
        if (attackTimer < Time.time) {
            if (attack) {
                Attack();
            }
        } else {

        }
    }

    private void Attack() {
        charAnim.SetTrigger(hashTAttack);
        attackTimer = Time.time + attackCooldown;
    }
}