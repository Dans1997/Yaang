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

    IEnumerator LoadScene(int buildIndex)
    {
        transitionAnimator.SetTrigger("loadLevel");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
