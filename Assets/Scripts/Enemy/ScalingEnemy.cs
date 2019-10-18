using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingEnemy : Enemy {
    [SerializeField] Transform graphics;
    [SerializeField] float startScale = 1f;
    [SerializeField] float multiplier = 0.5f;
    [SerializeField] HomingBullet bulletPrefab;
    [SerializeField] float minShootDist = 7500f;
    [SerializeField] float minShootDistToCenter = 5f;
    [SerializeField] float shootCooldown = 0.2f;

    private Transform playerRef;
    private Character charRef;
    private float shootTimer;

    private void Awake() {
        charRef = GetComponent<Character>();
    }

    private void Start() {
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null) playerRef = playerGO.transform;
    }

    private void LateUpdate() {
        if (playerRef == null) return;
        // Look at player
        transform.LookAt(playerRef);
        // Scale according to distance to player
        float distance = Vector3.Distance(playerRef.position, transform.position);
        graphics.localScale = Vector3.one * (distance * multiplier + startScale);
        // Check if cooldown has passed
        if (Time.time < shootTimer) return;
        // Shoot if below desired distance
        if (distance < minShootDist) {
            Shoot();
        }
    }

    private void Shoot() {
        // Create a random rotation because why not
        float rX = UnityEngine.Random.Range(0, 360);
        float rY = UnityEngine.Random.Range(0, 360);
        float rZ = UnityEngine.Random.Range(0, 360);
        Quaternion hbRotation = Quaternion.Euler(rX, rY, rZ);
        Vector3 hbPosition = transform.position + (hbRotation * Vector3.one * minShootDistToCenter);
        // Instantiate new bullet
        HomingBullet hb = Instantiate<HomingBullet>(bulletPrefab, transform.position, hbRotation);
        hb.Setup(charRef, playerRef);
        // Set cooldown
        shootTimer = Time.time + shootCooldown;
    }
}
