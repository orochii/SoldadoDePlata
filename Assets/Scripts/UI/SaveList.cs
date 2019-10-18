using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveList : MonoBehaviour {
    [System.Serializable]
    public class SaveEntry {
        public float x;
        public float y;
        public string name;
    }
    [SerializeField] private DrawableImageOptions saveFilePrefab;
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private Transform activeEntryContainer;
    private List<SaveEntry> entries;
    private List<GameObject> childEntries;
    private string filename = "savelist";
    private bool canSave;
    public bool CanSave { get { return canSave; } } //TODO: Deactivate save option

    public void Open(bool _canSave) {
        canSave = _canSave;
        Time.timeScale = 0;
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        gameObject.SetActive(true);
    }

    internal void SetToActive(Transform entry, bool v) {
        if (v) entry.SetParent(activeEntryContainer);
        else entry.SetParent(entriesContainer);
    }

    public void Close() {
        Time.timeScale = 1;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        gameObject.SetActive(false);
    }

    private void OnEnable() {
        // Destroy all children if there is any
        if (childEntries != null) {
            foreach (GameObject go in childEntries) Destroy(go);
        }
        RecreateEntries();
    }

    private void RecreateEntries() {
        // Create child entries holder if there is none
        if (childEntries == null) childEntries = new List<GameObject>();
        else childEntries.Clear();
        // Load save file data
        GameManager.CheckSaveFolder();
        if (GameManager.SaveExists(filename)) {
            LoadData();
        } else {
            entries = new List<SaveEntry>();
        }
        CreateExistingEntries();
    }

    private void CreateExistingEntries() {
        foreach(SaveEntry se in entries) {
            DrawableImageOptions entry = Instantiate<DrawableImageOptions>(saveFilePrefab, entriesContainer);
            RectTransform rt = (RectTransform)entry.transform;
            entry.savefile = se.name;
            rt.localPosition = new Vector3(se.x, se.y, 0);
            entry.selfEntry = se;
            entry.parentSaveList = this;
            entry.Init();
            childEntries.Add(entry.gameObject);
        }
    }

    public void CreateNewEntry() {
        SaveEntry se = new SaveEntry();
        se.name = "save" + entries.Count;
        entries.Add(se);
        DrawableImageOptions entry = Instantiate<DrawableImageOptions>(saveFilePrefab, entriesContainer);
        entry.savefile = se.name;
        entry.selfEntry = se;
        entry.parentSaveList = this;
        entry.Init();
        entry.SetEdit(true);
        childEntries.Add(entry.gameObject);
    }

    private void LoadData() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(GameManager.SaveFilename(filename), FileMode.Open);
        // Load data
        SaveEntry[] ary = (SaveEntry[])bf.Deserialize(file);
        entries = new List<SaveEntry>(ary);
        file.Close();
    }

    public void SaveData() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(GameManager.SaveFilename(filename));
        bf.Serialize(file, entries.ToArray());
        file.Close();
    }
}