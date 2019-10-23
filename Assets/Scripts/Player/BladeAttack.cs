using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeAttack : MonoBehaviour {
    [SerializeField] private float baseDamage = 0;
    [SerializeField] private Character.EDamageKind damageKind;
    [SerializeField] private Character user;
    [SerializeField] ParticleSystem[] collisionEffectPrefabs;
    [SerializeField] TrailRenderer trail;
    [SerializeField] AudioSource bladeSource;
    private bool active;
    public bool Active {
        get { return active; }
        set {
            trail.emitting = value;
            active = value;
            if (active == true) AudioManager.instance.PlaySoundFromSource(bladeSource, "whoosh");
        }
    }

    // PARTICLES: resist,damage,dead,recovery
    private void OnTriggerEnter(Collider other) {
        if (!active) return;
        if (!other.CompareTag(tag)) {
            Debug.Log(other.name);
            Character character = other.GetComponent<Character>();
            int particleIndex = 0;
            // Deal damage if the object has any character attached.
            if (character != null) {
                float dmg = baseDamage + user.GetAttack(damageKind) * 4;
                particleIndex = character.Damage(dmg, damageKind);
            }
            if (particleIndex < 0) particleIndex += collisionEffectPrefabs.Length;
            // Create effect
            if (collisionEffectPrefabs.Length > 0 && collisionEffectPrefabs[particleIndex] != null) {
                RaycastHit hit;
                Vector3 direction = (other.transform.position - transform.position).normalized;
                if (Physics.Raycast(transform.position, direction, out hit)) {
                    //hit.point;
                    ParticleSystem ps = Instantiate(collisionEffectPrefabs[particleIndex], hit.point, transform.rotation);
                    float d = ps.main.duration + 2; //ps.main.startLifetime.Evaluate(1);
                    Destroy(ps.gameObject, d);
                }
            }
        }
    }
}
