using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour {
    [SerializeField] BladeAttack meleeWeapon;

    public void SetEnable(int value) {
        meleeWeapon.Active = value==1;
    }
}
