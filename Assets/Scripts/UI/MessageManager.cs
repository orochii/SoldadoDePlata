using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour {
    private static MessageManager m_Instance;
    public static MessageManager Instance { get { return m_Instance; } }

    [SerializeField] TMPro.TextMeshProUGUI messageText;
    [SerializeField] GameObject messageWindow;
    private bool finished;
    private int currentLetters;
    private string currentText = "";
    private float wait;
    private float waitTimer;

    private void Awake() {
        m_Instance = this;
    }

    void Start() {
        messageText.SetText("");
        messageWindow.SetActive(false);
    }

    private void FixedUpdate() {
        if (finished) {
            if (Time.time > waitTimer) {
                UnsetText();
            }
            return;
        }
        if (currentLetters < currentText.Length) {
            currentLetters++;
            messageText.SetText(currentText.Substring(0, currentLetters));
        } else {
            waitTimer = Time.time + wait;
            finished = true;
        }
    }

    private void UnsetText() {
        messageText.SetText("");
        messageWindow.SetActive(false);
        currentText = "";
        currentLetters = 0;
        finished = false;
    }

    public void ShowText(string text, float _wait) {
        currentText = text;
        currentLetters = 0;
        messageWindow.SetActive(true);
        wait = _wait;
        finished = false;
    }
}