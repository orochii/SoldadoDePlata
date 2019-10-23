using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackController : MonoBehaviour {
    [SerializeField] private BladeAttack[] blades;
    [SerializeField] private ParticleSystem[] feetParticles;
    [SerializeField] private AudioSource footsteps;
    private bool isActive;
    public bool Active { get { return isActive; } }

    public void SetEnable(int v) {
        isActive = (v == 1); //attacker
        foreach (BladeAttack b in blades) b.Active = isActive;
    }

    public void EmitSoilParticles(int idx) {
        feetParticles[idx].Emit(30);
        AudioManager.instance.PlaySoundFromSource(footsteps, "steps");
    }
}
