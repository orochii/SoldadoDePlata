using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventPage : MonoBehaviour {
    public enum ECommand {
        SHOWTEXT, WAIT, TELEPORT, LOOKAT, LOOKATPLAYER, RESETDIRECTION, CALL, SAVE, SETSWITCH, PLAYSOUND
    }
    [System.Serializable]
    public class EventCommand {
        public ECommand kind;
        public string text;
        public int num;
        public int num2;
        public Vector3 position;
        public UnityEngine.Events.UnityEvent callback;
    }

    [SerializeField] private bool interact;
    [SerializeField] private EventCommand[] commands;
    [SerializeField] private Transform graphics;
    private bool closeToPlayer;
    private float startingDirection;

    private void Awake() {
        startingDirection = transform.rotation.eulerAngles.y;
    }

    private void Update() {
        if (GameEvent.Waiting) return;
        if (closeToPlayer && interact) {
            if (Input.GetButtonUp("Fire2")) StartEvent();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (GameEvent.Waiting) return;
        if (other.CompareTag("Player")) {
            if (interact) {
                closeToPlayer = true;
            }
            else StartEvent();
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            closeToPlayer = false;
        }
    }

    private void StartEvent() {
        StopAllCoroutines();
        StartCoroutine(ProcessEvent());
    }

    IEnumerator ProcessEvent() {
        GameEvent.Waiting = true;
        int index = 0;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        while (index < commands.Length) {
            EventCommand ec = commands[index];
            switch(ec.kind) {
                case ECommand.SHOWTEXT:
                    MessageManager.Instance.ShowText(ec.text, ec.num);
                    yield return new WaitForSeconds(ec.num2);
                    break;
                case ECommand.WAIT:
                    yield return new WaitForSeconds(ec.num);
                    break;
                case ECommand.TELEPORT:
                    Debug.Log("Not implemented :(");
                    break;
                case ECommand.LOOKAT:
                    Vector3 directionVector = ec.position - transform.position;
                    float direction = Vector3.SignedAngle(Vector3.forward, directionVector, Vector3.up);
                    if (graphics != null) graphics.rotation = Quaternion.Euler(0, direction, 0);
                    else transform.rotation = Quaternion.Euler(0, direction, 0);
                    break;
                case ECommand.LOOKATPLAYER:
                    if (player != null) {
                        Vector3 dV = player.transform.position - transform.position;
                        float d = Vector3.SignedAngle(Vector3.forward, dV, Vector3.up);
                        if (graphics != null) graphics.rotation = Quaternion.Euler(0, d, 0);
                        else transform.rotation = Quaternion.Euler(0, d, 0);
                    }
                    break;
                case ECommand.RESETDIRECTION:
                    transform.rotation = Quaternion.Euler(0, startingDirection, 0);
                    break;
                case ECommand.CALL:
                    if (ec.callback != null) ec.callback.Invoke();
                    break;
                case ECommand.SAVE:
                    if (player != null) GameManager.Instance.respawnPosition = player.transform.position;
                    GameManager.Instance.saveMenu.Open(true);
                    break;
                case ECommand.SETSWITCH:
                    GameManager.SetSwitch(ec.num, ec.num2 == 1);
                    break;
                case ECommand.PLAYSOUND:
                    Vector3 playPos = transform.position;
                    if (ec.num2 > 0) playPos = ec.position;
                    AudioManager.instance.PlaySound(ec.text, playPos, ec.num);
                    break;
            }
            index++;
            yield return null;
        }
        GameEvent.Waiting = false;
        GameEvent.CheckAllPages();
    }
}