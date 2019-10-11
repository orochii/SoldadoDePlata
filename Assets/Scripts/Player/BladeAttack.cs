using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeAttack : MonoBehaviour {
    [SerializeField] private float baseDamage = 0;
    [SerializeField] private Character.EDamageKind damageKind;
    [SerializeField] private Character user;
    [SerializeField] GameObject collisionEffectPrefab;
    [SerializeField] TrailRenderer trail;
    private bool active;
    public bool Active {
        get { return active; }
        set {
            trail.emitting = value;
            active = value;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag(tag)) {
            Character character = other.GetComponent<Character>();
            // Deal damage if the object has any character attached.
            if (character != null) {
                float dmg = baseDamage + user.GetAttack(damageKind) * 4;
                character.Damage(dmg, damageKind);
            }
            // Create effect
            if (collisionEffectPrefab != null) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit)) {
                    //hit.point;
                    Instantiate(collisionEffectPrefab, hit.point, transform.rotation);
                }
            }
        }
    }
}
