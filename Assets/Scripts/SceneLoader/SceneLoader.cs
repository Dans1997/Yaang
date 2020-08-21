using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Singleton

    private static SceneLoader _instance;

    public static SceneLoader SceneLoaderInstance
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

    private void Awake()
    {
        _instance = this;
    }

    #endregion Singleton

    // Cached Components
    Animator transitionAnimator;

    private void Start()
    {
        transitionAnimator = GetComponentInChildren<Animator>();
    }

    public void ReloadScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadScene(currentSceneIndex));
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadScene(currentSceneIndex + 1));
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadScene(0));
    }

    // If there is a save file, load scene that is not completed yet
    public void Continue()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        PlayerData playerData = SaveSystem.LoadPlayer();
        if (playerData != null)
        {
            for (int i = 0; i < playerData.levelStatistics.Count; i++)
            {
                //Debug.Log(playerData.levelStatistics[i].levelName + " " + playerData.levelStatistics[i].timesCompleted);
                if (playerData.levelStatistics[i].timesCompleted == 0 && playerData.levelStatistics[i].levelName.Contains("Level"))
                {
                    StartCoroutine(LoadSceneByName(playerData.levelStatistics[i].levelName));
                    break;
                }
            }
        }
        else
        {
            StartCoroutine(LoadScene(currentSceneIndex + 1));
        }
    }

    IEnumerator LoadScene(int buildIndex)
    {
        transitionAnimator.SetTrigger("loadLevel");
        AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.Transition);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(buildIndex);
    }

    IEnumerator LoadSceneByName(string sceneName)
    {
        transitionAnimator.SetTrigger("loadLevel");
        AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.Transition);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
