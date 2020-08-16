using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Cached Components 
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

        audioManager = AudioManager.AudioManagerInstance;
    }

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
