using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour {
    private static LoadingManager m_Instance = null;
    public static LoadingManager Instance { get { return m_Instance; } }

    [SerializeField] private string startingScene;
    [SerializeField] GameObject canvas;
    [SerializeField] RectTransform bar;
    [SerializeField] Image overlay;
    [SerializeField] TMPro.TMP_Text loadingText;
    [SerializeField] string loadingStr = "Cargando...";
    [SerializeField] string loadEndStr = "¡Listo!";
    
    private Coroutine _loadingRoutine;
    private float _barWidth;
    private string currentSceneName;
    public static string CurrentSceneName { get { return m_Instance.currentSceneName; } }
    public static bool IsLoading { get; private set; }

    void Awake() {
        DontDestroyOnLoad(gameObject);
        m_Instance = this;
        _barWidth = bar.rect.width;
        canvas.SetActive(false);
        ResizeBar(0);
        ChangeOverlayCol(0);
        Load(startingScene, false);
    }

    private void ChangeOverlayCol(float v) {
        overlay.color = new Color(0, 0, 0, v);
    }
    private void ResizeBar(float v) {
        float w = (1 - v) * _barWidth;
        bar.sizeDelta = new Vector2(-w, bar.sizeDelta.y);
    }

    public static void Load(string sceneName, bool disableOldScene=true) {
        if (GameManager.Instance != null) {
            GameManager.Instance.lastSceneName = sceneName;
        }
        Scene oldScene = SceneManager.GetActiveScene();
        Instance.LoadScene(sceneName, oldScene, disableOldScene);
    }

    private void LoadScene(string sceneName, Scene old, bool disableOldScene) {
        if (_loadingRoutine != null) {
            StopCoroutine(_loadingRoutine);
        }
        _loadingRoutine = StartCoroutine(LoadSceneRoutine(sceneName, old, disableOldScene));
    }

    public static void BasicLoad(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneRoutine(string sceneName, Scene old, bool disableOldScene) {
        canvas.SetActive(true);
        ResizeBar(0);
        float percent;
        loadingText.SetText(loadingStr);
        // Disable player
        PlayerControl pc = FindObjectOfType<PlayerControl>();
        if (pc != null) pc.SetBusy(true);
        // Set flag
        IsLoading = true;
        // Obscure the screen.
        percent = 0;
        while (percent < 1) {
            percent += Time.deltaTime;
            ChangeOverlayCol(percent);
            yield return null;
        }
        // Disable everything
        GameObject[] gos = old.GetRootGameObjects();
        if (disableOldScene) {
            foreach (GameObject go in gos) {
                go.SetActive(false);
            }
        }
        // Start loading the scene.
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        // Set current scene name.
        currentSceneName = sceneName;
        AudioManager.instance.AutoPlayMusic();
        // Wait and do stuff while it's in progress.
        while (!loadingScene.isDone) {
            // Change bar size.
            ResizeBar(loadingScene.progress);
            yield return null;
        }
        ResizeBar(1);
        loadingText.SetText(loadEndStr);
        // Destroy past scene.
        AsyncOperation unloadScene = SceneManager.UnloadSceneAsync(old);
        while (!unloadScene.isDone) {
            // TODO Show that it's done.
            yield return null;
        }
        // Fade to new scene.
        percent = 0;
        while (percent < 1) {
            percent += Time.deltaTime;
            ChangeOverlayCol(1 - percent);
            yield return null;
        }
        IsLoading = false;
        canvas.SetActive(false);
        yield return null;
    }

    public static void AddScene(string scene, System.Action callback) {
        Instance.AddSceneOperation(scene, callback);
    }

    internal static void CloseScene(string scene, System.Action callback) {
        Instance.CloseSceneOperation(scene, callback);
    }

    private void AddSceneOperation(string scene, System.Action callback) {
        StartCoroutine(DoAddScene(scene, callback));
    }
    private IEnumerator DoAddScene(string scene, System.Action callback) {
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        while (!loadingScene.isDone) yield return null;
        if (callback != null) callback();
    }
    private void CloseSceneOperation(string scene, System.Action callback) {
        StartCoroutine(DoCloseScene(scene, callback));
    }
    private IEnumerator DoCloseScene(string scene, System.Action callback) {
        AsyncOperation unloadingScene = SceneManager.UnloadSceneAsync(scene);
        while (!unloadingScene.isDone) yield return null;
        if (callback != null) callback();
    }
}