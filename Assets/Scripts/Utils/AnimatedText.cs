﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimatedText : MonoBehaviour {
    [SerializeField] AnimationCurve posCurve;
    [SerializeField] bool animateColor;
    [SerializeField] Gradient colorGradient;
    [SerializeField] float curveScale = 8;
    [SerializeField] float animationLength;
    
    private TMP_Text _text = null;
    private bool hasTextChanged = false;

    void Awake() {
        _text = GetComponent<TMP_Text>();
    }

    void OnEnable() {
        // Subscribe to event fired when text object has been regenerated.
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
    }
    void OnDisable() {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }
    void OnTextChanged(Object obj) {
        if (obj == _text)
            hasTextChanged = true;
    }

    void Start() {
        StartCoroutine(AnimateLetters());
    }

    private IEnumerator AnimateLetters() {
        _text.ForceMeshUpdate();
        TMP_TextInfo textInfo = _text.textInfo;
        hasTextChanged = true;
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        float animationStatus = 0;
        while (true) {
            if (hasTextChanged) {
                cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
                hasTextChanged = false;
            }
            int characterCount = textInfo.characterCount;
            if (characterCount == 0) {
                yield return new WaitForSeconds(0.25f);
                continue;
            }
            for (int i = 0; i < characterCount; i++) {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible)
                    continue;
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                float indPerc = (float)i / characterCount;
                float time = animationStatus + (indPerc / animationLength);
                if (time > 1) time -= 1;
                Vector3 offset = new Vector3(0, curveScale, 0) * posCurve.Evaluate(time);
                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + offset;
                if (animateColor) {
                    Color32 c = colorGradient.Evaluate(time);
                    Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                    newVertexColors[vertexIndex + 0] = c;
                    newVertexColors[vertexIndex + 1] = c;
                    newVertexColors[vertexIndex + 2] = c;
                    newVertexColors[vertexIndex + 3] = c;
                    _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                }
            }
            // Push changes into meshes
            for (int i = 0; i < textInfo.meshInfo.Length; i++) {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                _text.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
            animationStatus += 0.05f / animationLength;
            if (animationStatus > 1) animationStatus -= 1;
            yield return new WaitForSeconds(0.05f);
        }
    }
}