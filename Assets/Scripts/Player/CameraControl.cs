using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private float rotationSpeedX = 60;
    [SerializeField] private float rotationSpeedY = 60;
    [SerializeField] private Vector2 minMaxRotX = new Vector2(30,90);
    [SerializeField] private Transform pivot;
    [SerializeField] private bool mouseRotation;

    private Vector3 currentVelocity;
    private Camera cam;
    private Transform target;

    private float rotX;
    private float rotY;
    public float RotationY { get { return rotY; } }

    void Start() {
        cam = Camera.main;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        cam.transform.position = transform.position;
        cam.transform.rotation = transform.rotation;
        Vector3 ea = pivot.rotation.eulerAngles;
        rotX = ea.x;
        rotY = ea.y;
    }

    private void Update() {
        if (mouseRotation) {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            // Calculate new values for view rotation
            rotX = Mathf.Clamp(rotX + mouseY * rotationSpeedX, minMaxRotX.x, minMaxRotX.y);
            rotY += mouseX * rotationSpeedY;
        }
        // Submit rotation on pivot
        pivot.rotation = Quaternion.Euler(rotX, rotY, 0);
        // Submit player position on pivot
        if (target != null) pivot.position = target.position;
    }

    void LateUpdate() {
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, transform.position, ref currentVelocity, smoothTime);
        cam.transform.rotation = transform.rotation;
    }
}