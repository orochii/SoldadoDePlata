using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private const int SWITCHES_SIZE = 50;
    private const int VARIABLE_SIZE = 50;
    
    private static GameManager m_Instance;
    public static GameManager Instance { get { return m_Instance; } }
    
    public static bool GetSwitch(int id) {
        if (m_Instance == null) return false;
        if (m_Instance.state == null) return false;
        return m_Instance.state.switches[id];
    }
    public static void SetSwitch(int id, bool v) {
        if (m_Instance == null) return;
        if (m_Instance.state == null) return;
        m_Instance.state.switches[id] = v;
    }

    public static float GetVariable(int id) {
        if (m_Instance == null) return 0;
        if (m_Instance.state == null) return 0;
        return m_Instance.state.variables[id];
    }

    public static void SetVariable(int id, float v) {
        if (m_Instance == null) return;
        if (m_Instance.state == null) return;
        m_Instance.state.variables[id] = v;
    }

    [System.Serializable]
    public class GameState {
        public bool[] switches;
        public float[] variables;
        public Dictionary<float, MovingObject.State> objectStates;
        public string lastScene = "Main";

        public GameState () {
            switches = new bool[SWITCHES_SIZE];
            variables = new float[VARIABLE_SIZE];
            objectStates = new Dictionary<float, MovingObject.State>();
        }
    }
    public PhysicMaterial slideAsButter;
    public string lastSaveSlot = "save";
    public string lastSceneName = "Main";
    public Vector3 respawnPosition;
    public SaveList saveMenu;

    [SerializeField] private GameState state;

    void Awake() {
        if (m_Instance != null) {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void New() {
        state = new GameState();
    }

    public void SaveOnLastSlot() {
        Save(lastSaveSlot);
    }

    public void Save(string filename) {
        // Create save folder if necessary.
        CheckSaveFolder();
        // Clear previous data
        state.objectStates.Clear();
        // Serialize all objects
        MovingObject[] allObjects = GameObject.FindObjectsOfType<MovingObject>();
        foreach (MovingObject o in allObjects) {
            MovingObject.State os = o.CreateState();
            state.objectStates.Add(os.identifier, os);
        }
        // Remember last scene name.
        state.lastScene = lastSceneName;
        // Save data to disk
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(SaveFilename(filename));
        // Write data to file.
        bf.Serialize(file, state);

        file.Close();

    }
    public bool Load(string filename) {
        // Check if file exists
        if (SaveExists(filename)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(SaveFilename(filename), FileMode.Open);
            // Load data
            state = (GameState)bf.Deserialize(file);
            lastSceneName = state.lastScene;
            file.Close();
            return true;
        }
        return false;
    }

    public static bool SaveExists(string filename) {
        return File.Exists(SaveFilename(filename));
    }

    public static string SaveFilename(string filename) {
        string path = SaveFilePath();
        return Path.Combine(path, filename + ".dat");
    }

    public static string SaveFilePath() {
        return Path.Combine(Application.persistentDataPath, "Saves");
    }

    public static void CheckSaveFolder() {
        string path = SaveFilePath();
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }


    internal MovingObject.State GetObjData(float identifier) {
        if (state == null) return null;
        if (state.objectStates.ContainsKey(identifier)) return state.objectStates[identifier];
        return null;
    }
}
