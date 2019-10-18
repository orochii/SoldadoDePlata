using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivationObject : MovingObject {
    [System.Serializable]
    public class ActivationExtraData : StateExtraData {
        public bool active;
        public int index;

        public ActivationExtraData(bool _active, int _index) {
            active = _active;
            index = _index;
        }
    }
    [System.Serializable]
    public class MovementData {
        public Vector3 targetPosition;
        public Vector3 targetRotation;
        public float waitTime;
    }
    public enum EActivationKind {
        NONE, POS, ROT, BOTH
    }
    [SerializeField] private bool active;
    [SerializeField] EActivationKind kind;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private MovementData[] movementData; 
    [SerializeField] private bool looping;
    [SerializeField] private UnityEvent onActiveChange;
    [SerializeField] private AudioSource movingSoundSource;
    private bool waiting;
    private float waitTimer;
    private int positionIndex;
    private Vector3 deactivatedPosition;
    private Quaternion deactivatedRotation;
    private Coroutine activationCoroutine;
    public bool Active { get { return active; } }

    private void OnDrawGizmosSelected() {
        if (movementData == null) return;
        Gizmos.color = Color.green;
        foreach(MovementData md in movementData) {
            Gizmos.matrix = Matrix4x4.TRS(md.targetPosition, Quaternion.Euler(md.targetRotation), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, 30, .5f, .001f, 1);
        }
    }

    private void Awake() {
        if (active) {
            positionIndex = movementData.Length - 1;
            waiting = true;
        }
        deactivatedPosition = transform.position;
        deactivatedRotation = transform.rotation;
    }

    protected override void OnLoadData(State s) {
        base.OnLoadData(s);
        ActivationExtraData aed = (ActivationExtraData)s.extraData;
        active = aed.active;
        positionIndex = aed.index % movementData.Length;
        if (active) {
            if (kind == EActivationKind.POS || kind == EActivationKind.BOTH) {
                transform.position = movementData[positionIndex].targetPosition;
            }
            if (kind == EActivationKind.ROT || kind == EActivationKind.BOTH) {
                transform.rotation = Quaternion.Euler(movementData[positionIndex].targetRotation);
            }
            waiting = true;
        } else {
            positionIndex = 0;
        }
    }

    private void Update() {
        if (active && waiting) {
            if (waitTimer < Time.time) {
                // Advance index
                positionIndex++;
                if (positionIndex >= movementData.Length) {
                    if (looping) positionIndex = 0;
                    else {
                        waiting = false;
                        return;
                    }
                }
                Vector3 p = transform.position;
                Quaternion r = transform.rotation;
                if (kind == EActivationKind.POS || kind == EActivationKind.BOTH) {
                    p = movementData[positionIndex].targetPosition;
                }
                if (kind == EActivationKind.ROT || kind == EActivationKind.BOTH) {
                    r = Quaternion.Euler(movementData[positionIndex].targetRotation);
                }
                if (activationCoroutine != null) StopCoroutine(activationCoroutine);
                activationCoroutine = StartCoroutine(GoTowardsDestination(p, r));
                waiting = false;
            }
        }
    }

    public void ToggleActive() {
        SetActive(!active);
    }

    public void SetActive(bool a) {
        if (a == active) return;
        active = a;
        Vector3 p = transform.position;
        Quaternion r = transform.rotation;
        if (kind == EActivationKind.POS || kind == EActivationKind.BOTH) {
            p = active ? movementData[positionIndex].targetPosition : deactivatedPosition;
        }
        if (kind == EActivationKind.ROT || kind == EActivationKind.BOTH) {
            r = active ? Quaternion.Euler(movementData[positionIndex].targetRotation) : deactivatedRotation;
        }
        if (!active) positionIndex = 0;
        if (activationCoroutine != null) StopCoroutine(activationCoroutine);
        activationCoroutine = StartCoroutine(GoTowardsDestination(p,r));
        // Send activation message
        if (onActiveChange != null) onActiveChange.Invoke();
    }
    public IEnumerator GoTowardsDestination(Vector3 position, Quaternion rotation, bool next=false) {
        float moveSpeedF = moveSpeed * Time.fixedDeltaTime;
        float rotateSpeedF = rotateSpeed * Time.fixedDeltaTime;
        if (movingSoundSource != null) movingSoundSource.Play();
        while (position != transform.position || rotation != transform.rotation) {
            transform.position = Vector3.MoveTowards(transform.position, position, moveSpeedF);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeedF);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        if (movingSoundSource != null) movingSoundSource.Stop();
        waiting = true;
        waitTimer = Time.time + movementData[positionIndex].waitTime;
    }

    protected override void OnCreateExtraData(State s) {
        base.OnCreateExtraData(s);
        s.extraData = new ActivationExtraData(active, positionIndex);
    }
}
