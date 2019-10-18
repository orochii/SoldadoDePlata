using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour {
    [SerializeField] private float radius = 1;
    public float Radius { get { return radius; } }
    private Rigidbody rbody;
    private Collider[] colliders;
    private float _mass;
    private float _drag;
    private float _angularDrag;
    private bool _useGravity;

    void Awake() {
        rbody = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void SetGrabbed(bool v) {
        //rbody.isKinematic = v;
        //rbody.detectCollisions = !v;
        if (v) {
            _mass = rbody.mass;
            _drag = rbody.drag;
            _angularDrag = rbody.angularDrag;
            _useGravity = rbody.useGravity;
            Destroy(rbody);
            SetPhysMaterial(GameManager.Instance.slideAsButter);
        } else {
            rbody = gameObject.AddComponent<Rigidbody>();
            rbody.mass = _mass;
            rbody.drag = _drag;
            rbody.angularDrag = _angularDrag;
            rbody.useGravity = _useGravity;
            SetPhysMaterial(null);
        }
    }
    
    private void SetPhysMaterial(PhysicMaterial pMat) {
        foreach (Collider c in colliders) {
            c.material = pMat;
        }
    }

    public void AddForce(Vector3 force) {
        rbody.AddForce(force);
    }
}
