using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatus : MonoBehaviour {
    [SerializeField] Image hpBar;
    [SerializeField] Image spBar;
    Character player;

    void Start() {
        GameObject pGO = GameObject.FindGameObjectWithTag("Player");
        if (pGO != null) player = pGO.GetComponent<Character>();
    }

    void Update() {
        if (player == null) return;
        float hpPerc = player.HP / player.MaxHP;
        float spPerc = player.SP / player.MaxSP;
        hpBar.fillAmount = hpPerc;
        spBar.fillAmount = spPerc;
    }
}