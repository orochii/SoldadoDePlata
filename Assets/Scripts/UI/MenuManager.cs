using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {
    public void New() {
        GameManager.Instance.New();
        LoadingManager.Load("Main");
    }
    public void Load() {
        if (GameManager.Instance.Load("save")) {
            LoadingManager.Load("Main");
        }
    }
    public void Exit() {
        Application.Quit();
    }
}
