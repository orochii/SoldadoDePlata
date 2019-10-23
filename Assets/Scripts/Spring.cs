using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {
    [SerializeField] private float springSpeed = 10f;
    [SerializeField] private float springCooldown = 0.5f;
    [SerializeField] private AudioSource springSound;
    private Animator anim;
    private int hashTSpring;
    private float springTimer;

    void Awake() {
        anim = GetComponent<Animator>();
        hashTSpring = Animator.StringToHash("spring");
    }

    private void OnTriggerEnter(Collider other) {
        if (springTimer > Time.time) return;
        Vector3 v = other.attachedRigidbody.velocity;
        v.y = springSpeed;
        other.attachedRigidbody.velocity = v;
        springTimer = Time.time + springCooldown;
        anim.SetTrigger(hashTSpring);
        springSound.Play();
    }
}