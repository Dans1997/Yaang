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

    [System.Serializable]
    public class Level
    {
        public int levelId = 0; //buildIndex
        public int timesCompleted = 0;
        public int timesFailed = 0;

        public bool isFirstVisit = true;
    }

    // Reference to all the scenes
    [SerializeField] List<Level> levels = new List<Level>();

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
            SceneLoader.SceneLoaderInstance.LoadNextScene();
        }
        else
        {
            Debug.LogWarning("Scene with id " + id + " not found!");
            return;
        }
    }
}
