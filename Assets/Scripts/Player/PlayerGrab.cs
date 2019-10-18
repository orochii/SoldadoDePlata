using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour {
    [SerializeField] private Transform playerPivot;
    [SerializeField] private float grabCooldown;
    [SerializeField] private float throwForce = 50;
    [SerializeField] private Transform[] hands;
    [SerializeField] private float distanceMult = 0.8f;

    private Grabbable objectInHand;
    public bool IsGrabbing { get { return objectInHand != null; } }
    private float grabTimer;
    private float objectDistance;

    private void Update() {
        UpdatePosition();
    }

    private void UpdatePosition() {
        Vector3 averagePosition = new Vector3();
        // Get average position for hands
        foreach (Transform h in hands) averagePosition += (h.position - playerPivot.position);
        if (hands.Length > 0) averagePosition /= hands.Length;
        averagePosition = Quaternion.Inverse(playerPivot.rotation) * averagePosition.normalized;
        Vector3 posRotation = averagePosition;
        float angle = Vector3.SignedAngle(Vector3.forward, posRotation, Vector3.right);
        float l = 1; //Mathf.Abs(angle / 90);
        float d = (objectDistance + l * distanceMult);
        posRotation *= d;
        transform.localPosition = posRotation;
        transform.localRotation = Quaternion.Euler(angle,0,0);
    }

    public void Grab(Grabbable toGrab) {
        if (grabTimer > Time.time) return;
        objectInHand = toGrab;
        objectInHand.transform.SetParent(transform);
        objectDistance = toGrab.Radius; //Vector3.Distance(playerPivot.position, objectInHand.transform.position);
        objectInHand.SetGrabbed(true);
        objectInHand.transform.localPosition = Vector3.zero;
        UpdatePosition();
    }

    public void Toss() {
        if (grabTimer > Time.time) return;
        objectInHand.transform.SetParent(null);
        objectInHand.SetGrabbed(false);
        objectInHand.AddForce(playerPivot.forward * throwForce);
        objectInHand = null;
    }
}