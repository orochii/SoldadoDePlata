using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlickering : MonoBehaviour {
    [SerializeField] private AnimationCurve flickeringSettings;
    private Light lightSource;
    private float _originalIntensity;

    private float timer;

    void Start() {
        lightSource = GetComponent<Light>();
        _originalIntensity = lightSource.intensity;
    }

    void FixedUpdate() {
        lightSource.intensity = _originalIntensity * flickeringSettings.Evaluate(timer);
        timer += Time.fixedDeltaTime;
        if (timer > 1) timer -= 1;
    }
}