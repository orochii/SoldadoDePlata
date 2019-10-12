using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationObject : MovingObject {
    [System.Serializable]
    public class ActivationExtraData : StateExtraData {
        public bool active;

        public ActivationExtraData(bool _active) {
            active = _active;
        }
    }
    public enum EActivationKind {
        NONE, POS, ROT, BOTH
    }
    [SerializeField] private bool active;
    [SerializeField] EActivationKind kind;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 targetRotation;
    private Vector3 deactivatedPosition;
    private Quaternion deactivatedRotation;
    private Coroutine activationCoroutine;

    private void Awake() {
        deactivatedPosition = transform.position;
        deactivatedRotation = transform.rotation;
    }

    protected override void OnLoadData(State s) {
        base.OnLoadData(s);
        active = ((ActivationExtraData)s.extraData).active;
        if (active) {
            if (kind == EActivationKind.POS || kind == EActivationKind.BOTH) {
                transform.position = targetPosition;
            }
            if (kind == EActivationKind.ROT || kind == EActivationKind.BOTH) {
                transform.rotation = Quaternion.Euler(targetRotation);
            }
        }
    }

    public void SetActive(bool a) {
        if (a == active) return;
        active = a;
        Vector3 p = transform.position;
        Quaternion r = transform.rotation;
        if (kind == EActivationKind.POS || kind == EActivationKind.BOTH) {
            p = active ? targetPosition : deactivatedPosition;
        }
        if (kind == EActivationKind.ROT || kind == EActivationKind.BOTH) {
            r = active ? Quaternion.Euler(targetRotation) : deactivatedRotation;
        }
        if (activationCoroutine != null) StopCoroutine(activationCoroutine);
        activationCoroutine = StartCoroutine(GoTowardsDestination(p,r));
    }
    public IEnumerator GoTowardsDestination(Vector3 position, Quaternion rotation) {
        float moveSpeedF = moveSpeed * Time.fixedDeltaTime;
        float rotateSpeedF = rotateSpeed * Time.fixedDeltaTime;
        while (position != transform.position || rotation != transform.rotation) {
            transform.position = Vector3.MoveTowards(transform.position, position, moveSpeedF);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeedF);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    protected override void OnCreateExtraData(State s) {
        base.OnCreateExtraData(s);
        s.extraData = new ActivationExtraData(active);
    }
}
