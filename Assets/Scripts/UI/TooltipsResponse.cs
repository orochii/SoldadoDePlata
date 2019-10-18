using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipsResponse : MonoBehaviour {
    [SerializeField] Image attackBack;
    [SerializeField] Image grabBack;
    [SerializeField] Color highlightColor;
    [SerializeField] float fadeLerp = 0.5f;
    private Color _defaultColor;

    private void Awake() {
        _defaultColor = attackBack.color;
    }

    private void Update() {
        bool attack = Input.GetButton("Fire1");
        bool grab = Input.GetButton("Fire2");
        // Lerp towards default color
        attackBack.color = Color.Lerp(attackBack.color, _defaultColor, fadeLerp);
        grabBack.color = Color.Lerp(grabBack.color, _defaultColor, fadeLerp);
        // Set color to highlight if corresponding button is pressed
        if (attack) attackBack.color = highlightColor;
        if (grab) grabBack.color = highlightColor;
    }
}
