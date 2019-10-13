using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour {
    private Rigidbody rbody;
    private float _mass;
    private float _drag;
    private float _angularDrag;

    void Awake() {
        rbody = GetComponent<Rigidbody>();
    }

    public void SetGrabbed(bool v) {
        //rbody.isKinematic = v;
        //rbody.detectCollisions = !v;
        if (v) {
            _mass = rbody.mass;
            _drag = rbody.drag;
            _angularDrag = rbody.angularDrag;
            Destroy(rbody);
        } else {
            rbody = gameObject.AddComponent<Rigidbody>();
            rbody.mass = _mass;
            rbody.drag = _drag;
            rbody.angularDrag = _angularDrag;
        }
    }
    public void AddForce(Vector3 force) {
        rbody.AddForce(force);
    }
}
