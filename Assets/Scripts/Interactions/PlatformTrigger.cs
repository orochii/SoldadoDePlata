using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour {
    [SerializeField] private PlatformBehaviour parentBehaviour;
    
    private void OnTriggerEnter(Collider other) {
        parentBehaviour.OnChildTriggerEnter(other, this);
    }
    private void OnTriggerExit(Collider other) {
        parentBehaviour.OnChildTriggerExit(other, this);
    }
}
