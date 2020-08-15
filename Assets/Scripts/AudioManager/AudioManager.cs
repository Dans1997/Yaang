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

        TileLightDown1,

        Footstep,
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

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            return;
        }

        SetUpSounds();

        // Main Theme
        PlaySound(SoundKey.MainTheme, transform.position);
        // Transition Sound
        PlaySound(SoundKey.Transition);
    }

    private void SetUpSounds()
    {
        foreach (Sound sound in gameSounds)
        {
            if (sound.delay > 0f)
            {
                soundTimerDictionary[sound.soundKey] = 0f;
            }
        }
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
                    oneShotObj = new GameObject("Audio Manager One Shot Object");
                    oneShotAudioSource = oneShotObj.AddComponent<AudioSource>();
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

    public void PlaySound(SoundKey soundKey, Vector3 position)
    {
        Sound sound = System.Array.Find(gameSounds, s => s.soundKey == soundKey);
        if (sound != null)
        {
            if (CanPlaySound(sound))
            {
                GameObject soundGameObject = new GameObject("Audio Manager 3D Object");
                soundGameObject.transform.position = position;
                AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

                // Audio Source Config
                audioSource.clip = sound.clip;
                audioSource.volume = sound.volume;
                audioSource.pitch = sound.pitch;
                audioSource.spatialBlend = sound.spatialBlend;
                audioSource.loop = sound.loop;

                if (sound.loop)
                {
                    audioSource.Play();
                }
                else
                {
                    audioSource.Play();
                    Object.Destroy(soundGameObject, audioSource.clip.length);
                    // TODO: To improve performace: Object Pooling
                }
            }
        }
        else
        {
            Debug.LogError("Sound " + soundKey + "not found!");
        }
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
