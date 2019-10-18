using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour {
    [SerializeField] private LayerMask layers;
    [SerializeField] private Transform collisionPivot;
    private float distance;

    private void Start() {
        distance = Vector3.Distance(transform.position, collisionPivot.position);
        UpdatePivotOffset();
    }

    private void LateUpdate() {
        UpdatePivotOffset();
    }

    private void UpdatePivotOffset() {
        collisionPivot.localPosition = Quaternion.Inverse(transform.rotation) * Vector3.up * distance;
    }

    private bool IsInLayerMask(int layer) {
        return (((layers >> layer) & 1) == 1);
    }

    internal void OnChildTriggerEnter(Collider other, PlatformTrigger child) {
        if (IsInLayerMask(other.gameObject.layer)) {
            if (other.transform.parent == null) {
                other.transform.SetParent(transform);
            }
        }
    }
    internal void OnChildTriggerExit(Collider other, PlatformTrigger child) {
        if (IsInLayerMask(other.gameObject.layer)) {
            if (other.transform.parent == transform) {
                other.transform.SetParent(null);
            }
        }
    }

    /*
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
    }*/
}
