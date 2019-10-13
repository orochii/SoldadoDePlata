using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour {
    [SerializeField] private LayerMask layers;

    private bool IsInLayerMask(int layer) {
        return (((layers >> layer) & 1) == 1);
    }

    private void OnTriggerEnter(Collider other) {
        if (IsInLayerMask(other.gameObject.layer)) {
            if (other.transform.parent == null) {
                other.transform.SetParent(transform);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (IsInLayerMask(other.gameObject.layer)) {
            if (other.transform.parent == transform) {
                other.transform.SetParent(null);
            }
        }
    }
}
