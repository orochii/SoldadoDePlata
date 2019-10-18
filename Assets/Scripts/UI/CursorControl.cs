using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICursorControlHover {
    void OnHoverUpdate(bool v);
}

public class CursorControl : MonoBehaviour {
    public static Vector3 Position { get; private set; }

    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float moveSpeedSlow = 2f;
    [SerializeField] private float dragThreshold = 0.33f;
    [SerializeField] private string button = "Fire1";
    [SerializeField] private string slowButton = "Fire2";
    private bool isDragging;
    private float dragTimer;
    private CursorResponse draggingElement;
    private ICursorControlHover lastHover;
    
    void Update() {
        // Move and update cursor position
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        bool slow = !Input.GetButton(slowButton);
        float _moveSpeed = slow ? moveSpeed : moveSpeedSlow;
        UpdatePosition(mouseX * _moveSpeed, mouseY * _moveSpeed);
        // Hover update
        GameObject hoverGo = Utils.UIHelper.GetElementUnder(Position);
        if (hoverGo != null) {
            ICursorControlHover currentHover = hoverGo.GetComponent<ICursorControlHover>();
            if (lastHover != currentHover) {
                if (lastHover != null) lastHover.OnHoverUpdate(false);
                if (currentHover != null) currentHover.OnHoverUpdate(true);
                lastHover = currentHover;
            }
        }
        // Get events
        bool bDown = Input.GetButtonDown(button);
        bool bPress = Input.GetButton(button);
        bool bUp = Input.GetButtonUp(button);
        if (bDown) {
            dragTimer = Time.unscaledTime + dragThreshold;
            GameObject go = Utils.UIHelper.GetElementUnder(Position);
            if (go != null) draggingElement = go.GetComponent<CursorResponse>();
        }
        if (bPress) {
            if (Time.unscaledTime > dragTimer) {
                if (isDragging) Drag();
                else BeginDrag();
            }
        }
        if (bUp) {
            if (isDragging) EndDrag();
            else PointerClick();
            isDragging = false;
        }
    }

    private void PointerClick() {
        GameObject go = Utils.UIHelper.GetElementUnder(Position);
        if (go != null) {
            CursorResponse uiElement = go.GetComponent<CursorResponse>();
            if (uiElement != null) {
                uiElement.OnPointerClick();
            }
        }
    }

    private void BeginDrag() {
        isDragging = true;
        if (draggingElement == null) return;
        draggingElement.OnBeginDrag();
    }
    private void Drag() {
        if (draggingElement == null) return;
        draggingElement.OnDrag();
    }
    private void EndDrag() {
        if (draggingElement == null) return;
        draggingElement.OnEndDrag();
    }

    private void UpdatePosition(float mouseX, float mouseY) {
        float newX = Mathf.Clamp(transform.position.x + mouseX, 0, Screen.width);
        float newY = Mathf.Clamp(transform.position.y + mouseY, 0, Screen.height);
        transform.position = new Vector3(newX, newY);
        Position = transform.position;
    }
}
