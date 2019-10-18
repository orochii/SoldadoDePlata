using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class StateExtraData {

}

public class MovingObject : MonoBehaviour {
    // Serializable state for moving objects
    [System.Serializable]
    public class State {
        public float identifier;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public StateExtraData extraData;
        // Access
        public Vector3 position {
            get {
                return new Vector3(positionX, positionY, positionZ);
            }
            set {
                positionX = value.x;
                positionY = value.y;
                positionZ = value.z;
            }
        }
        public Vector3 rotation {
            get {
                return new Vector3(rotationX, rotationY, rotationZ);
            }
            set {
                rotationX = value.x;
                rotationY = value.y;
                rotationZ = value.z;
            }
        }
    }
    
    private float identifier = 0;
    [SerializeField] private string collideScenerySound = "thump";
    [SerializeField] private AudioSource selfAudioSource;

    void Start() {
        identifier = transform.position.sqrMagnitude;
        State s = GameManager.Instance.GetObjData(identifier);
        if (s != null) {
            transform.position = s.position;
            transform.rotation = Quaternion.Euler(s.rotation);
            OnLoadData(s);
        }
    }

    public State CreateState() {
        State s = new State();
        s.identifier = identifier;
        s.position = transform.position;
        s.rotation = transform.rotation.eulerAngles;
        OnCreateExtraData(s);
        return s;
    }

    protected virtual void OnLoadData(State s) {

    }
    protected virtual void OnCreateExtraData(State s) {

    }

    private void OnCollisionEnter(Collision collision) {
        //if (collision.collider.gameObject.layer == 9) {
            if (selfAudioSource != null) AudioManager.instance.PlaySoundFromSource(selfAudioSource, collideScenerySound);
        //}
    }

}