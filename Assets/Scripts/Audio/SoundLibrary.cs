using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundLibrary : MonoBehaviour {

    public SoundGroup[] soundGroups;

    Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();

    void Awake() {
        foreach (SoundGroup soundGroup in soundGroups) {
            groupDictionary.Add(soundGroup.groupID, soundGroup.group);
        }
    }

    public AudioClip GetClipFromName(string name, int index = -1) {
        if (groupDictionary.ContainsKey(name)) {
            AudioClip[] sounds = groupDictionary[name];
            if (index < 0) index = Random.Range(0, sounds.Length);
            if (index < sounds.Length) return sounds[index];
        }
        return null;
    }

    [System.Serializable]
    public class SoundGroup {
        public string groupID;
        public AudioClip[] group;
    }
}
