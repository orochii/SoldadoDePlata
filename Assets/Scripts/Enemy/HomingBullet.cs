using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour {
    [SerializeField] float startingSpeed = 5;
    [SerializeField] float acceleration = 5;
    [SerializeField] float maxSpeed = 80;
    [SerializeField] float rotationSpeed = 5;
    [SerializeField] float baseDamage = 10;
    [SerializeField] float maxLife = 10;
    [SerializeField] ParticleSystem collisionEffectPrefab;
    private Character parent;
    private Transform target;

    private float speed;
    private Vector3 velocity;

    private void RandomizeData() {
        startingSpeed *= UnityEngine.Random.Range(.5f, 2f);
        acceleration *= UnityEngine.Random.Range(.5f, 2f);
        maxSpeed *= UnityEngine.Random.Range(.5f, 2f);
        rotationSpeed *= UnityEngine.Random.Range(.01f, 1f);
        maxLife *= UnityEngine.Random.Range(.1f, 1f);
    }

    private void Start() {
        speed = startingSpeed;
        Destroy(gameObject, maxLife);
    }

    private void FixedUpdate() {
        // Rotate towards target
        Vector3 lookPos = target.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotationSpeed);
        // Update speed
        speed = Mathf.Clamp(speed + Time.fixedDeltaTime * acceleration, -maxSpeed, maxSpeed);
        velocity = transform.forward * speed;
        // Commit movement
        transform.position += velocity;
    }

    public void Setup(Character _parent, Transform _target) {
        parent = _parent;
        target = _target;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Character otherChar = other.GetComponent<Character>();
            if (otherChar != null) {
                float dmg = baseDamage + parent.GetAttack(Character.EDamageKind.MAGICAL) * 4;
                otherChar.Damage(dmg, Character.EDamageKind.MAGICAL);
            }
            // Create effect
            if (collisionEffectPrefab != null) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit)) {
                    //hit.point;
                    ParticleSystem ps = Instantiate(collisionEffectPrefab, hit.point, transform.rotation);
                    float d = ps.main.duration + 2; //ps.main.startLifetime.Evaluate(1);
                    Destroy(ps.gameObject, d);
                }
            }
        }
        Destroy(gameObject);
    }
}