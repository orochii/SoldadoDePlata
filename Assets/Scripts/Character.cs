using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MovingObject {
    public const float minHeight = -200f;

    public enum EDamageKind {
        NONE, PHYSICAL, MAGICAL
    }
    [SerializeField] private float maxHP = 100;
    [SerializeField] private float maxSP = 100;
    [SerializeField] private float physAttack = 1;
    [SerializeField] private float physDefense = 1;
    [SerializeField] private float magiAttack = 1;
    [SerializeField] private float magiDefense = 1;
    [SerializeField] UnityEvent onLoad;
    [SerializeField] UnityEvent onDead;
    [SerializeField] UnityEvent onRevive;
    [SerializeField] UnityEvent onFall;
    [SerializeField] private float currHP;
    [SerializeField] private float currSP;
    [SerializeField] private bool dead;
    public bool Busy;
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
                    if (currHP <= 0) currHP = 1;
                    if (onRevive != null) onRevive.Invoke();
                }
            }
        }
    }
    public float HP {
        get { return currHP; }
        set {
            currHP = Mathf.Clamp(value, 0, maxHP);
            if (currHP == 0) Dead = true;
        }
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

    private void Awake() {
        currHP = maxHP;
        currSP = maxSP;
    }
    protected override void OnLoadData(MovingObject.State s) {
        base.OnLoadData(s);
        CharacterExtraData extra = (CharacterExtraData)s.extraData;
        currHP = extra.currHP;
        currSP = extra.currSP;
        maxHP = extra.maxHP;
        maxSP = extra.maxSP;
        physAttack = extra.physAttack;
        physDefense = extra.physDefense;
        magiAttack = extra.magiAttack;
        magiDefense = extra.magiDefense;
        dead = extra.dead;
        if (onLoad != null) onLoad.Invoke();
    }

    void Update() {
        if (dead) return;
        if (Busy) return;
        float h = transform.position.y;
        if (h < minHeight) if (onFall != null) onFall.Invoke();
    }

    public void Die() {
        HP = 0;
    }

    public void Revive() {
        Dead = false;
        currHP = maxHP;
    }
    [System.Serializable]
    public class CharacterExtraData : StateExtraData {
        public float currHP;
        public float currSP;
        public float maxHP;
        public float maxSP;
        public float physAttack;
        public float physDefense;
        public float magiAttack;
        public float magiDefense;
        public bool dead;
        
    }

    protected override void OnCreateExtraData(State s) {
        base.OnCreateExtraData(s);
        CharacterExtraData extra = new CharacterExtraData();
        extra.currHP = currHP;
        extra.currSP = currSP;
        extra.maxHP = maxHP;
        extra.maxSP = maxSP;
        extra.physAttack = physAttack;
        extra.physDefense = physDefense;
        extra.magiAttack = magiAttack;
        extra.magiDefense = magiDefense;
        extra.dead = dead;
        s.extraData = extra;
    }

}
