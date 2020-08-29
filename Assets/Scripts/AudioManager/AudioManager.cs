using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    private static AudioManager _instance;

    public static AudioManager AudioManagerInstance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Audio Manager is null!");
            }

            return _instance;
        }
    }

    #endregion Singleton

    public enum SoundKey
    {
        PlayerMove,
        PlayerPower,
        PlayerDeath,
        PlayerGroundHit,

        MainTheme,
        Transition,
        Button,

        TileLightUp1,
        TileLightUp2,
        TileLightUp3,
        TileLightUp4,

        FireTileWarmUp,
        FireTileLightUp1,

        TileLightDown1,

        Footstep,

        Reboot,

        CyberTile1,
        CyberTileZap1,

        Teleport1,
        Teleport2,
    }

    [System.Serializable]
    public class Sound
    {
        [HideInInspector] public AudioSource audioSource;
        public SoundKey soundKey;
        public AudioClip clip;

        // AudioSource Config
        [Range(0f, 10f)] public float delay;
        [Range(0f, 1f)] public float volume;
        [Range(0f, 3f)] public float pitch;
        [Range(0f, 1f)] public float spatialBlend;
        public bool loop;
    }

    // Game Sound Effects
    public Sound[] gameSounds;

    // Timers For The Sounds
    Dictionary<SoundKey, float> soundTimerDictionary = new Dictionary<SoundKey, float>();

    // Cached Components
    GameObject oneShotObj;
    AudioSource oneShotAudioSource;

    // Pooling System
    int queueCount = 15;
    Queue<AudioSource> audioObjectsQueue3D = new Queue<AudioSource>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetUpSounds();
    }

    private void SetUpSounds()
    {
        // Set Up One Shot Audio Source
        if (oneShotObj == null)
        {
            oneShotObj = new GameObject("Audio Manager One Shot Object");
            oneShotObj.transform.parent = transform;
            oneShotAudioSource = oneShotObj.AddComponent<AudioSource>();
        }

        // Set 3D AudioSources Queue
        while(audioObjectsQueue3D.Count < queueCount)
        {
            GameObject soundGameObject = new GameObject("Audio Manager 3D Object");
            soundGameObject.transform.position = transform.position;
            soundGameObject.transform.parent = transform;
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            soundGameObject.SetActive(false);

            audioObjectsQueue3D.Enqueue(audioSource);
        }


        foreach (Sound sound in gameSounds)
        {
            if (sound.delay > 0f)
            {
                soundTimerDictionary[sound.soundKey] = 0f;
            }
        }

        // Main Theme
        PlaySound(SoundKey.MainTheme, transform.position);

        // Transition Sound
        PlaySound(SoundKey.Transition);
    }

    public void PlaySelectButton() => PlaySound(SoundKey.Button);

    // Ignores Looping and SpatialBlend
    public void PlaySound(SoundKey soundKey)
    {
        Sound sound = System.Array.Find(gameSounds, s => s.soundKey == soundKey);
        if (sound != null)
        {
            if (CanPlaySound(sound))
            {
                if (oneShotObj == null)
                {
                    Debug.LogWarning("One Shot Audio Object is Null! Returning...");
                    return;
                }

                // Audio Source Config
                oneShotAudioSource.volume = sound.volume;
                oneShotAudioSource.pitch = sound.pitch;
                oneShotAudioSource.spatialBlend = 0f;
                oneShotAudioSource.loop = false;

                oneShotAudioSource.PlayOneShot(sound.clip, sound.volume);
            }
        }
        else
        {
            Debug.LogError("Sound " + soundKey + "not found!");
        }
    }

    public AudioSource PlaySound(SoundKey soundKey, Vector3 position)
    {
        Sound sound = System.Array.Find(gameSounds, s => s.soundKey == soundKey);
        if (sound != null)
        {
            if (CanPlaySound(sound))
            {
                if(audioObjectsQueue3D.Count <= 0)             
                {
                    return null;
                }

                AudioSource audioSource = audioObjectsQueue3D.Dequeue();
                audioSource.gameObject.SetActive(true);

                // Audio Source Config
                audioSource.clip = sound.clip;
                audioSource.volume = sound.volume;
                audioSource.pitch = sound.pitch;
                audioSource.spatialBlend = sound.spatialBlend;
                audioSource.loop = sound.loop;
                audioSource.dopplerLevel = 0f;

                if (sound.loop)
                {
                    audioSource.Play();
                    return audioSource;
                }
                else
                {
                    audioSource.Play();
                    StartCoroutine(Return3DSourceToPool(audioSource));
                    return audioSource;
                }
            }
            else return null;
        }
        else
        {
            Debug.LogError("Sound " + soundKey + "not found!");
            return null;
        }
    }

    IEnumerator Return3DSourceToPool(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.gameObject.SetActive(false);
        audioObjectsQueue3D.Enqueue(audioSource);
    }

    private bool CanPlaySound(Sound sound)
    {
        // If the sound loops, the delay is ignored
        if (sound.loop) return true;

        if (!soundTimerDictionary.ContainsKey(sound.soundKey)) return true;

        float lastTimePlayed = soundTimerDictionary[sound.soundKey];

        if (lastTimePlayed + sound.delay < Time.time)
        {
            soundTimerDictionary[sound.soundKey] = Time.time;
            return true;
        }
        else return false;
    }
}
