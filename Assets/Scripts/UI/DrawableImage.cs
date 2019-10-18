using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.IO;

public class DrawableImage : MonoBehaviour, ICursorControlHover {
    private const int WIDTH = 64;
    private const int HEIGHT = 64;

    private Image _self;
    private Texture2D tex;
    private bool active = false;
    public Color CurrentColor = Color.black;
    public int CurrentSize = 3;

    void Awake() {
        tex = new Texture2D(WIDTH, HEIGHT);
        Color[] cols = new Color[WIDTH * HEIGHT];
        for (int i = 0; i < cols.Length; i++) cols[i] = Color.white;
        tex.SetPixels(cols);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        _self = GetComponent<Image>();
        _self.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    private void Update() {
        if (!active) return;
        bool fire1 = Input.GetButton("Fire1");
        bool fire2 = Input.GetButton("Fire2");
        if (fire1 || fire2) {
            Color32 col = fire1 ? CurrentColor : Color.white;
            Rect r = _self.rectTransform.rect;
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_self.rectTransform, CursorControl.Position, Camera.main, out localPoint)) {
                int px = Mathf.Clamp(0, (int)(((localPoint.x - r.x) * tex.width) / r.width), tex.width);
                int py = Mathf.Clamp(0, (int)(((localPoint.y - r.y) * tex.height) / r.height), tex.height);
                PrintOnCanvas(px, py, col, CurrentSize);
                tex.Apply();
            }
        }
    }

    private void PrintOnCanvas(int x, int y, Color c, int size) {
        x -= size / 2;
        y -= size / 2;
        for (int cx = 0; cx < size; cx++) {
            int xx = x + cx;
            for (int cy = 0; cy < size; cy++) {
                int yy = y + cy;
                if (ValidPosition(xx,yy)) tex.SetPixel(xx, yy, c);
            }
        }
    }

    private bool ValidPosition(int x, int y) {
        return (x >= 0 && y >= 0 && x < WIDTH && y < HEIGHT);
    }

    public void Save(string filename) {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(filename, bytes);
    }

    public void Load(string filename) {
        if (File.Exists(filename)) {
            byte[] bytes = File.ReadAllBytes(filename);
            tex.LoadImage(bytes);
        }
    }

    public void OnHoverUpdate(bool v) {
        active = v;
    }
}
