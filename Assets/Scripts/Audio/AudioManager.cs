using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public enum AudioChannel {  Master, Sfx, Bgm };
    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float bgmVolumePercent { get; private set; }

    [SerializeField] bool test_deactivateMusic;
    [SerializeField] bool test_deactivateSound;

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourdeIndex;

    public static AudioManager instance;

    Transform audioListener;
    Transform cameraT;

    SoundLibrary library;
    MusicManager music;

    void Awake() {
        if(instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
            library = GetComponent<SoundLibrary>();
            music = GetComponent<MusicManager>();

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++) {
                GameObject newMusicSource = new GameObject("Music Source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
                musicSources[i].loop = true;
            }
            GameObject newSfx2DSource = new GameObject("2D SFX Source");
            sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
            newSfx2DSource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            
            //cameraT = Camera.main.transform;

            masterVolumePercent = PlayerPrefs.GetFloat("Audio Master Volume", 1);
            sfxVolumePercent = PlayerPrefs.GetFloat("Audio SFX Volume", 1);
            bgmVolumePercent = PlayerPrefs.GetFloat("Audio BGM Volume", 1);
        }
    }

    public void SetVolume(float newVolume, AudioChannel channel) {
        switch(channel) {
            case AudioChannel.Master:
                masterVolumePercent = newVolume;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = newVolume;
                break;
            case AudioChannel.Bgm:
                bgmVolumePercent = newVolume;
                break;
        }
        
        musicSources[0].volume = GetBgmVolume();
        musicSources[1].volume = GetBgmVolume();

        PlayerPrefs.SetFloat("Audio Master Volume", masterVolumePercent);
        PlayerPrefs.SetFloat("Audio SFX Volume", sfxVolumePercent);
        PlayerPrefs.SetFloat("Audio BGM Volume", bgmVolumePercent);
        PlayerPrefs.Save();
    }

    void Update() {
        if(cameraT != null) {
            audioListener.position = cameraT.position;
        } else {
            Camera c = Camera.main;
            if (c != null) cameraT = c.transform;
        }
    }

    public AudioClip CurrentMusic() {
        return musicSources[activeMusicSourdeIndex].clip;
    }

    public void AutoPlayMusic() {
        music.AutoPlayMusic();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1) {
        if (test_deactivateMusic) return;
        if (musicSources[activeMusicSourdeIndex].clip == clip) return;
        activeMusicSourdeIndex = 1 - activeMusicSourdeIndex;
        if (clip == null) musicSources[activeMusicSourdeIndex].Stop();
        else {
            musicSources[activeMusicSourdeIndex].clip = clip;
            musicSources[activeMusicSourdeIndex].Play();
        }

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos) {
        if(clip != null) {
            AudioSource.PlayClipAtPoint(clip, pos, GetSfxVolume());
        }
        
    }

    public void PlaySound(string soundName, Vector3 pos, int index = -1) {
        PlaySound(library.GetClipFromName(soundName, index), pos);
    }

    public void PlaySoundFromSource(AudioSource source, string soundName, int index = -1) {
        AudioClip clip = library.GetClipFromName(soundName, index);
        source.PlayOneShot(clip);
    }

    public void PlaySound2D(AudioClip clip) {
        if (clip == null) return;
        sfx2DSource.PlayOneShot(clip, GetSfxVolume());
    }

    public void PlaySound2D(string soundName, int index = -1) {
        PlaySound2D(library.GetClipFromName(soundName, index));
    }

    public float GetBgmVolume() {
        return bgmVolumePercent * masterVolumePercent * .5f;
    }
    public float GetSfxVolume() {
        return sfxVolumePercent * masterVolumePercent;
    }

    IEnumerator AnimateMusicCrossfade(float duration) {
        float percent = 0;
        while(percent < 1) {
            percent += Time.unscaledDeltaTime * 1 / duration;
            musicSources[activeMusicSourdeIndex].volume = Mathf.Lerp(0, GetBgmVolume(), percent);
            musicSources[1-activeMusicSourdeIndex].volume = Mathf.Lerp(GetBgmVolume(), 0, percent);
            yield return null;
        }
    }
}
