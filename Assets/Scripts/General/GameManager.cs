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

    [System.Serializable]
    public class GameState {
        public bool[] switches;
        public float[] variables;
        public Dictionary<float, MovingObject.State> objectStates;

        public GameState () {
            switches = new bool[SWITCHES_SIZE];
            variables = new float[VARIABLE_SIZE];
            objectStates = new Dictionary<float, MovingObject.State>();
        }
    }
    private GameState state;

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
