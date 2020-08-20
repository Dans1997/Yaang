using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _instance;

    public static GameManager GameManagerInstance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Game Manager is null!");
            }

            return _instance;
        }
    }

    #endregion Singleton

    public class Level
    {
        public int levelId = 0; //buildIndex
        public int timesCompleted = 0;
        public int timesFailed = 0;

        public bool isFirstVisit = true;
    }

    // Reference to all the scenes
    List<Level> levels = new List<Level>();

    // State
    bool wantsToReboot = false;

    // Cached Components 
    RebootAdsScript rebootAds;
    AudioManager audioManager;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            Level newLevel = new Level();
            newLevel.levelId = i;
            levels.Add(newLevel);
        }

        rebootAds = FindObjectOfType<RebootAdsScript>();
        audioManager = AudioManager.AudioManagerInstance;
    }

    #region LEVEL_REBOOT_HANDLER

    private void LateUpdate()
    {
        HandleReboot();
    }

    private void HandleReboot()
    {
        bool previousWantsToReboot = wantsToReboot;
        wantsToReboot = Input.GetKey(KeyCode.R) || CrossPlatformInputManager.GetButton("Reboot");
        if (wantsToReboot && !previousWantsToReboot)
        {
            StartCoroutine(RebootHandler());
            //TODO: SET REBOOT BUTTON UP BEFORE REBOOTING
        }
    }

    IEnumerator RebootHandler()
    {
        Camera mainCamera = Camera.main;
        float currentOrthoSize = mainCamera.orthographicSize;

        AudioSource rebootAudioSource = AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.Reboot, transform.position);
        for (float t = 0f; t < 3f; t += Time.deltaTime)
        {
            if (!wantsToReboot)
            {
                rebootAudioSource.Stop();
                break;
            }
            mainCamera.orthographicSize -= Time.deltaTime * 0.5f;
            yield return 0;
        }
        if (wantsToReboot) RebootLevel();
        else mainCamera.orthographicSize = currentOrthoSize;
    }

    public void RebootLevel()
    {
        int id = SceneManager.GetActiveScene().buildIndex;
        SetFirstVisit(id, true);
        rebootAds.ShowRebootAd();
    }

    #endregion

    public bool IsFirstVisit(int id)
    {
        if(levels[id] != null)
        {
            return levels[id].isFirstVisit;
        }
        else
        {
            Debug.LogWarning("Id " + id + " is not a valid level index!");
            return false;
        }
    }

    public void SetFirstVisit(int id, bool firstVisit)
    {
        if (levels[id] != null)
        {
            levels[id].isFirstVisit = firstVisit;
        }
        else
        {
            Debug.LogWarning("Id " + id + " is not a valid level index!");
            return;
        }
    }

    public void CompleteLevel()
    {
        int id = SceneManager.GetActiveScene().buildIndex;
        if (levels[id] != null)
        {
            levels[id].timesCompleted++;
            StartCoroutine(CompletionCutscene());
        }
        else
        {
            Debug.LogWarning("Scene with id " + id + " not found!");
            return;
        }
    }

    IEnumerator CompletionCutscene()
    {
        Camera mainCamera = Camera.main;
        Transform playerPos = FindObjectOfType<PlayerController>().transform;
        PlayerAnimation playerAnimation = playerPos.GetComponentInChildren<PlayerAnimation>();
        playerAnimation.PlayAnimation("Move_Up_Loop");
        for (float t = 0f; t < 1f; t += Time.deltaTime)
        {
            mainCamera.orthographicSize += Time.deltaTime * 3;
            playerPos.position = new Vector3(playerPos.position.x, playerPos.position.y + Time.deltaTime * 3, playerPos.position.z);
            audioManager.PlaySound(AudioManager.SoundKey.Footstep);
            yield return 0;
        }
        // Move player to door?
        // Queue transition?
        yield return new WaitForSeconds(0);
        SceneLoader.SceneLoaderInstance.LoadNextScene();
    }

    public void FailLevel()
    {
        int id = SceneManager.GetActiveScene().buildIndex;
        if (levels[id] != null)
        {
            levels[id].timesFailed++;
            SceneLoader.SceneLoaderInstance.ReloadScene();
        }
        else
        {
            Debug.LogWarning("Scene with id " + id + " not found!");
            return;
        }
    }
}
