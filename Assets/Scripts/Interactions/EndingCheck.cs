using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCheck : MonoBehaviour {
    [SerializeField] ScalingEnemy boss;
    [SerializeField] float waitingTime = 20f;
    [SerializeField] float resetTimer = 20f;
    [SerializeField] GameObject endingCanvas;

    public bool CheckCondition(bool a) {
        return (!boss.enabled && !a);
    }

    public void OnCheckActivation(ActivationObject ao) {
        if (CheckCondition(ao.Active)) {
            StopAllCoroutines();
            StartCoroutine(DoEndingCinematic());
        }
    }

    IEnumerator DoEndingCinematic() {
        yield return new WaitForSeconds(0.1f);
        CameraControl cc = FindObjectOfType<CameraControl>();
        PlayerControl pc = FindObjectOfType<PlayerControl>();
        // Stop player, set camera
        cc.SetCustomTarget(transform);
        pc.SetBusy(true);
        GameEvent.Waiting = true;
        // Wait
        yield return new WaitForSeconds(waitingTime);
        endingCanvas.SetActive(true);
        yield return new WaitForSeconds(resetTimer);
        endingCanvas.SetActive(false);
        // Reset player, unset camera
        cc.SetCustomTarget(null);
        pc.SetBusy(false);
        GameEvent.Waiting = false;
    }
}
