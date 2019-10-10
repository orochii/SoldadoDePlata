using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour {
    public enum EDamageKind {
        NONE, PHYSICAL, MAGICAL
    }

    [SerializeField] private float maxHP = 100;
    [SerializeField] private float maxSP = 100;
    [SerializeField] private float physAttack = 1;
    [SerializeField] private float physDefense = 1;
    [SerializeField] private float magiAttack = 1;
    [SerializeField] private float magiDefense = 1;
    [SerializeField] UnityEvent onDead;
    [SerializeField] UnityEvent onRevive;
    [SerializeField] private float currHP;
    [SerializeField] private float currSP;
    [SerializeField] private bool dead;
    public float MaxHP { get { return maxHP; } }
    public float MaxSP { get { return maxSP; } }

    public bool Dead { get { return dead; }
        set {
            bool change = (dead != value);
            dead = value;
            if (change) {
                Debug.Log("Deaded");
                if (value) {
                    if (onDead != null) onDead.Invoke();
                } else {
                    if (onRevive != null) onRevive.Invoke();
                }
            }
        }
    }
    public float HP {
        get { return currHP; }
        set { currHP = Mathf.Clamp(value, 0, maxHP); }
    }
    public float SP {
        get { return currSP; }
        set { currSP = Mathf.Clamp(value, 0, maxSP); }
    }

    public void Damage(float baseDmg, EDamageKind damageKind) {
        if (dead) return;
        bool isRecovery = baseDmg < 0;
        float damage = baseDmg;
        if (!isRecovery) {
            float def = GetDefense(damageKind);
            damage -= (def / 2);
            if (damage <= 0) damage = 1;
        }
        // Debug.Log(name + " receives " + damage);
        HP -= damage;
        if (currHP == 0) Dead = true;
    }

    public float GetAttack(EDamageKind damageKind) {
        switch (damageKind) {
            case EDamageKind.PHYSICAL:
                return physAttack;
            case EDamageKind.MAGICAL:
                return magiAttack;
            default:
                return 0;
        }
    }

    public float GetDefense(EDamageKind damageKind) {
        switch (damageKind) {
            case EDamageKind.PHYSICAL:
                return physDefense;
            case EDamageKind.MAGICAL:
                return magiDefense;
            default:
                return 0;
        }
    }

    void Start() {
        currHP = maxHP;
        currSP = maxSP;
    }

    void Update() {

    }
}
