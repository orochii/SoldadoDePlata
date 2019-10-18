using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    private static InputManager m_Instance;
    public static InputManager Instance { get { return m_Instance; } }

    private const float MOUSE_SENSITIVITY_MIN = 0.1f;
    private const float MOUSE_SENSITIVITY_MAX = 1f;

    [SerializeField] [Range(MOUSE_SENSITIVITY_MIN, MOUSE_SENSITIVITY_MAX)] private float mouseSensitivity = 0.5f;

    public static float MouseSensitivity { get { return m_Instance.mouseSensitivity; }
        set {
            m_Instance.mouseSensitivity = Mathf.Clamp(value, MOUSE_SENSITIVITY_MIN, MOUSE_SENSITIVITY_MAX);
        }
    }

    public void Awake() {
        m_Instance = this;
        mouseSensitivity = PlayerPrefs.GetFloat("Mouse Sensitivity", 0.5f);
    }
    public void OnApplicationQuit() {
        PlayerPrefs.SetFloat("Mouse Sensitivity", mouseSensitivity);
    }
}
