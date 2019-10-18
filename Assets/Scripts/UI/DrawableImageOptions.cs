using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawableImageOptions : MonoBehaviour {
    [SerializeField] DrawableImage target;
    [SerializeField] Image preview;
    [SerializeField] GameObject interactOverlay;
    [SerializeField] GameObject editOverlay;
    [SerializeField] GameObject editDisabler;
    [SerializeField] int minSize = 1;
    [SerializeField] int maxSize = 16;
    [SerializeField] Button saveButton;
    public string savefile = "save";
    public SaveList parentSaveList;
    public SaveList.SaveEntry selfEntry;
    private bool editing;

    public void Init() {
        LoadImage();
        UpdatePreview();
        //saveButton.interactable = parentSaveList.CanSave;
        saveButton.gameObject.SetActive(parentSaveList.CanSave);
    }

    public void SetEdit(bool v) {
        interactOverlay.SetActive(!v);
        editOverlay.SetActive(v);
        editDisabler.SetActive(v);
        editing = v;
        parentSaveList.SetToActive(transform, v);
    }

    private void UpdatePreview() {
        preview.transform.localScale = Vector3.one * target.CurrentSize;
        preview.color = target.CurrentColor;
    }

    public void SetColor(Image origin) {
        target.CurrentColor = origin.color;
        UpdatePreview();
    }

    public void ChangeSize(int v) {
        target.CurrentSize = Mathf.Clamp(target.CurrentSize + v, minSize, maxSize);
        UpdatePreview();
    }
    
    public void Save() {
        GameManager.CheckSaveFolder();
        string filename = GameManager.SaveFilename(savefile) + ".png";
        target.Save(filename);
        GameManager.Instance.Save(savefile);
        SetEdit(false);
        parentSaveList.SaveData();
    }
    public void LoadImage() {
        GameManager.CheckSaveFolder();
        string filename = GameManager.SaveFilename(savefile) + ".png";
        target.Load(filename);
    }

    public void Load() {
        // TODO
        GameManager.Instance.Load(savefile);
        LoadingManager.Load(GameManager.Instance.lastSceneName);
        GameEvent.Waiting = false;
        parentSaveList.Close();
    }

    private Vector3 startingMousePosition;
    private Vector3 startingElementPosition;
    public void BeginDrag() {
        if (editing) return;
        startingMousePosition = CursorControl.Position;
        startingElementPosition = transform.position;
    }
    public void Drag() {
        if (editing) return;
        Vector3 deltaPosition = CursorControl.Position - startingMousePosition;
        transform.position = startingElementPosition + deltaPosition;
        Debug.Log(deltaPosition);
    }
    public void EndDrag() {
        if (editing) return;
        selfEntry.x = transform.localPosition.x;
        selfEntry.y = transform.localPosition.y;
    }
}
