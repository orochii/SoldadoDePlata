using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    [SerializeField] private float rotationSpeedX = 60;
    [SerializeField] private float rotationSpeedY = 60;
    [SerializeField] private Vector2 minMaxRotX = new Vector2(30,90);
    [SerializeField] private Transform rig;
    [SerializeField] private Transform pivot;
    [SerializeField] private bool mouseRotation;
    [SerializeField] private float minDistance = 1;
    [SerializeField] private float maxDistance = 5;
    [SerializeField] private float rigRadius = 0.02f;
    [SerializeField] private LayerMask cameraCollMask;

    private Transform customTarget;
    public void SetCustomTarget(Transform ct) {
        customTarget = ct;
    }
    public void UnsetCustomTarget() {
        customTarget = null;
    }
    private Vector3 currentVelocity;
    private Camera cam;
    private Transform target;

    private float rotX;
    private float rotY;
    public float RotationY { get { return rotY; } }
    public Quaternion Rotation { get { return transform.rotation; } }

    void Start() {
        cam = Camera.main;
        cam.tag = "Untagged";
        target = GameObject.FindGameObjectWithTag("Player").transform;
        cam.transform.position = transform.position;
        cam.transform.rotation = transform.rotation;
        Vector3 ea = pivot.rotation.eulerAngles;
        rotX = ea.x;
        rotY = ea.y;
    }

    private void FixedUpdate() {
        if (customTarget != null) return;
        if (mouseRotation) {
            float mouseX = Input.GetAxis("Mouse X") * InputManager.MouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * InputManager.MouseSensitivity;
            // Calculate new values for view rotation
            rotX = Mathf.Clamp(rotX + mouseY * rotationSpeedX, minMaxRotX.x, minMaxRotX.y);
            rotY += mouseX * rotationSpeedY;
        }
        // Submit rotation on pivot
        pivot.rotation = Quaternion.Euler(rotX, rotY, 0);
    }

    private void LateUpdate() {
        if (customTarget != null) {
            cam.transform.position = customTarget.position;
            cam.transform.rotation = customTarget.rotation;
            return;
        }
        // Collision check
        Vector3 direction = -pivot.forward;
        Ray ray = new Ray(pivot.position + direction * minDistance, direction);
        RaycastHit hit;
        float hitDist = maxDistance;
        bool cast = Physics.BoxCast(ray.origin, Vector3.one * 0.1f, ray.direction, out hit, transform.rotation, maxDistance - minDistance, cameraCollMask);
        // Physics.Raycast(ray, out hit, maxDistance - minDistance, cameraCollMask)
        if (cast) {
            hitDist = hit.distance + minDistance - rigRadius;
        }
        transform.localPosition = new Vector3(0, 0, -hitDist);
        // Submit player position on rig
        if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target != null) rig.position = target.position;
        // Submit movement onto camera object
        cam.transform.position = transform.position;
        cam.transform.rotation = transform.rotation;
    }
}