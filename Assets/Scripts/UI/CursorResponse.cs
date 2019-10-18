using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CursorResponse : MonoBehaviour {
    [SerializeField] private UnityEvent onPointerClick;
    [SerializeField] private UnityEvent onBeginDrag;
    [SerializeField] private UnityEvent onDrag;
    [SerializeField] private UnityEvent onEndDrag;

    public void OnPointerClick() {
        if (onPointerClick != null) onPointerClick.Invoke();
    }
    public void OnBeginDrag() {
        if (onBeginDrag != null) onBeginDrag.Invoke();
    }
    public void OnDrag() {
        if (onDrag != null) onDrag.Invoke();
    }
    public void OnEndDrag() {
        if (onEndDrag != null) onEndDrag.Invoke();
    }
}
