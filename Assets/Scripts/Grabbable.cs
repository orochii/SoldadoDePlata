using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour {
    private Rigidbody rbody;

    void Awake() {
        rbody = GetComponent<Rigidbody>();
    }

    public void SetKinematic(bool v) {
        rbody.isKinematic = v;
        rbody.detectCollisions = !v;
    }
    public void AddForce(Vector3 force) {
        rbody.AddForce(force);
    }
}
