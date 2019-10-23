using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WalkingEnemy : MonoBehaviour {
    [System.Serializable]
    public class PatrolData {
        public Vector3 position;
        public float time;
    }
    [SerializeField] private float sightDistance = 2f;
    [SerializeField] private float followDistance = 5f;
    [SerializeField] private float pursuitCooldown = 0.5f;
    [SerializeField] private Vector3 sightOffset = new Vector3(0,.5f,0);
    [SerializeField] private LayerMask sightBlock;
    [SerializeField] private PatrolData[] patrolDatas;
    [SerializeField] private Animator anim;
    [SerializeField] private MeleeAttackController attacker;
    [SerializeField] private float attackStartCooldown = 1f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float respawnCooldown = 20f;

    private Character selfChar;
    private Rigidbody rbody;
    private NavMeshAgent navAgent;
    private PlayerControl player;
    private Character playerHealth;
    private bool pursue;
    private float pursueTimer;
    private bool patrolArrived;
    private float patrolTimer;
    private float attackTimer;
    private int patrolIndex;
    private int hashF_MoveSpeed;
    private int hashT_Attack;
    private Vector3 startingPosition;
    //
    private Coroutine fallCoroutine;

    private void Awake() {
        startingPosition = transform.position;
    }

    private void Start() {
        player = FindObjectOfType<PlayerControl>();
        playerHealth = FindObjectOfType<Character>();
        navAgent = GetComponent<NavMeshAgent>();
        selfChar = GetComponent<Character>();
        rbody = GetComponent<Rigidbody>();
        patrolIndex = 0;
        hashF_MoveSpeed = Animator.StringToHash("movespeed");
        hashT_Attack = Animator.StringToHash("attack");
    }

    public void OnDead() {
        // Cancel all attacks, set movement to 0
        anim.SetFloat(hashF_MoveSpeed, 0);
        anim.Play("Death");
        attacker.SetEnable(0);
        rbody.velocity = Vector3.zero;
        navAgent.enabled = false;
        transform.position += Vector3.one * 0.1f;
        Respawn();
    }
    public void OnRevive() {
        anim.Play("Standby");
        rbody.velocity = Vector3.zero;
        navAgent.enabled = true;
        navAgent.SetDestination(transform.position);
    }

    public void Respawn() {
        if (fallCoroutine != null) StopCoroutine(fallCoroutine);
        fallCoroutine = StartCoroutine(DoRespawn());
    }

    private IEnumerator DoRespawn() {
        yield return new WaitForSeconds(respawnCooldown);
        if (selfChar != null) selfChar.Busy = true;
        yield return new WaitForSeconds(.5f);
        GoToRespawn();
        selfChar.Revive();
        yield return new WaitForSeconds(.5f);
        if (selfChar != null) selfChar.Busy = false;
    }

    public void GoToRespawn() {
        transform.position = startingPosition;
    }

    private void FixedUpdate() {
        if (selfChar.Dead) return;
        anim.SetFloat(hashF_MoveSpeed, navAgent.velocity.magnitude);
        // 
        if (attacker.Active) return;
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (pursue) {
            // Update rotation towards character
            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            float angle = Vector3.SignedAngle(Vector3.forward, playerDirection, Vector3.up);
            Quaternion newRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, navAgent.angularSpeed * Time.fixedDeltaTime);
            // Do attack if enough time has passed
            if (attackTimer < Time.time) {
                DoAttack();
            }
            // Check if player is visible
            if (dist < followDistance) {
                // Check if there is sometime between player and enemy.
                Ray ray = new Ray(transform.position + sightOffset, playerDirection);
                bool cast = Physics.Raycast(ray, dist, sightBlock);
                if (!cast) {
                    // Follow player
                    navAgent.SetDestination(player.transform.position);
                    // Reset timer
                    pursueTimer = Time.time + pursuitCooldown;
                }
            }
            if (Time.time > pursueTimer) {
                navAgent.updateRotation = true;
                pursue = false;
            }
        } else {
            if (dist < sightDistance) {
                Ray ray = new Ray(transform.position, (player.transform.position - transform.position).normalized);
                if (!Physics.Raycast(ray, dist, sightBlock)) {
                    navAgent.updateRotation = false;
                    pursue = true;
                    attackTimer = Time.time + attackStartCooldown;
                }
            } else {
                UpdatePatrolling();
            }
        }
    }

    private void DoAttack() {
        anim.SetTrigger(hashT_Attack);
        attackTimer = Time.time + attackCooldown;
    }

    private void UpdatePatrolling() {
        // If there is no patrol data, return.
        if (patrolDatas.Length == 0) return;
        // If agent hasn't finished moving.
        if (navAgent.remainingDistance > navAgent.stoppingDistance) return;
        else {
            // If finished, mark it as arrived. And wait N time.
            if (!patrolArrived) {
                patrolTimer = Time.time + patrolDatas[patrolIndex].time;
                patrolArrived = true;
            }
            // After it waits, start another movement, unmark as arrived.
            if (patrolTimer < Time.time) {
                patrolIndex = (patrolIndex + 1) % patrolDatas.Length;
                navAgent.SetDestination(patrolDatas[patrolIndex].position);
                patrolArrived = false;
            }
        }
    }

    private void OnDrawGizmosSelected() {
        // Draw detection distances
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, sightDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followDistance);
        // Draw patrolling positions
        foreach (PatrolData pd in patrolDatas) {
            Gizmos.DrawSphere(pd.position, 0.1f);
        }
    }
}
