using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour {
    private static bool waiting;
    public static bool Waiting { get { return waiting; } }

    public enum ECommand {
        SHOWTEXT, WAIT, TELEPORT, LOOKAT, LOOKATPLAYER, RESETDIRECTION, CALL, SAVE
    }
    [System.Serializable]
    public class EventCommand {
        public ECommand kind;
        public string text;
        public int num;
        public Vector3 position;
        public UnityEngine.Events.UnityEvent callback;
    }

    [SerializeField] private EventCommand[] commands;
    private float startingDirection;

    private void Start() {
        startingDirection = transform.rotation.eulerAngles.y;
    }

    private void OnTriggerEnter(Collider other) {
        if (waiting) return;
        if (other.CompareTag("Player")) {
            StartEvent();
        }
    }

    private void StartEvent() {
        StopAllCoroutines();
        StartCoroutine(ProcessEvent());
    }

    IEnumerator ProcessEvent() {
        waiting = true;
        int index = 0;
        while (index < commands.Length) {
            EventCommand ec = commands[index];
            switch(ec.kind) {
                case ECommand.SHOWTEXT:
                    MessageManager.Instance.ShowText(ec.text, ec.num);
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
                    transform.rotation = Quaternion.Euler(0, direction, 0);
                    break;
                case ECommand.LOOKATPLAYER:
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null) {
                        Vector3 dV = player.transform.position - transform.position;
                        float d = Vector3.SignedAngle(Vector3.forward, dV, Vector3.up);
                        transform.rotation = Quaternion.Euler(0, d, 0);
                    }
                    
                    break;
                case ECommand.RESETDIRECTION:
                    transform.rotation = Quaternion.Euler(0, startingDirection, 0);
                    break;
                case ECommand.CALL:
                    if (ec.callback != null) ec.callback.Invoke();
                    break;
                case ECommand.SAVE:
                    GameManager.Instance.Save("save");
                    break;
            }
            index++;
            yield return null;
        }
        waiting = false;
    }
}