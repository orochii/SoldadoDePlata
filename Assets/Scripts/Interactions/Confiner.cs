using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confiner : MonoBehaviour {
    [SerializeField] private string confinementTag;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(confinementTag)) {
            Enemy e = other.GetComponent<Enemy>();
            if (e != null) {
                e.enabled = false;
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(confinementTag)) {
            Enemy e = other.GetComponent<Enemy>();
            if (e != null) {
                e.enabled = true;
            }
        }
    }
}
