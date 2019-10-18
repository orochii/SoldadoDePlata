using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
    
    [System.Serializable]
    public class SceneMusic {
        public string sceneName;
        public AudioClip clip;
    }

    public SceneMusic[] music;

    string sceneName;

    public void AutoPlayMusic() {
        string newSceneName = LoadingManager.CurrentSceneName;
        PlayMusicByName(newSceneName);
    }
    public void AutoPlayMusic(string sceneName) {
        PlayMusicByName(sceneName);
    }

    void PlayMusicByName(string name) {
        if (name != sceneName) {
            sceneName = name;
            Invoke("PlayMusic", .2f);
        }
    }

    void PlayMusic() {
        AudioClip clipToPlay = null;
        
        foreach (SceneMusic sm in music) {
            if (sceneName == sm.sceneName) clipToPlay = sm.clip;
        }

        if(clipToPlay != null) {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            //Invoke("PlayMusic", clipToPlay.length);
        }

    }
}
