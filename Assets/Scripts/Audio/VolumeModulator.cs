using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeModulator : MonoBehaviour {
    [SerializeField] AudioSource sound;
    [SerializeField] float volumeMultiplier = 1;

	void Awake () {
        RefreshVolume();
    }

    void FixedUpdate () {
        RefreshVolume();
    }

    void RefreshVolume() {
        sound.volume = volumeMultiplier * AudioManager.instance.GetSfxVolume();
    }
}
