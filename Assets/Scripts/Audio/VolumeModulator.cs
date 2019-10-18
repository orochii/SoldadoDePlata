using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeModulator : MonoBehaviour {
    [SerializeField] AudioSource sound;

	void Awake () {
        RefreshVolume();
    }

    void FixedUpdate () {
        RefreshVolume();
    }

    void RefreshVolume() {
        sound.volume = AudioManager.instance.GetSfxVolume();
    }
}
