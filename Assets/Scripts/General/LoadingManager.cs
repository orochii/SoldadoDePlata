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

    void Awake() {
        DontDestroyOnLoad(gameObject);
        m_Instance = this;
        _barWidth = bar.rect.width;
        canvas.SetActive(false);
        ResizeBar(0);
        ChangeOverlayCol(0);
        Load(startingScene);
    }

    private void ChangeOverlayCol(float v) {
        overlay.color = new Color(0, 0, 0, v);
    }
    private void ResizeBar(float v) {
        float w = (1 - v) * _barWidth;
        bar.sizeDelta = new Vector2(-w, bar.sizeDelta.y);
    }

    public static void Load(string sceneName) {
        Scene oldScene = SceneManager.GetActiveScene();
        Instance.LoadScene(sceneName, oldScene);
    }

    private void LoadScene(string sceneName, Scene old) {
        if (_loadingRoutine != null) {
            StopCoroutine(_loadingRoutine);
        }
        _loadingRoutine = StartCoroutine(LoadSceneRoutine(sceneName, old));
    }

    public static void BasicLoad(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneRoutine(string sceneName, Scene old) {
        canvas.SetActive(true);
        ResizeBar(0);
        float percent;
        loadingText.SetText(loadingStr);
        // Obscure the screen.
        percent = 0;
        while (percent < 1) {
            percent += Time.deltaTime;
            ChangeOverlayCol(percent);
            yield return null;
        }
        // Start loading the scene.
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        // Wait and do stuff while it's in progress.
        while (!loadingScene.isDone) {
            // Change bar size.
            ResizeBar(loadingScene.progress);
            yield return null;
        }
        ResizeBar(1);
        loadingText.SetText(loadEndStr);
        // When it's finished, destroy past scene.
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