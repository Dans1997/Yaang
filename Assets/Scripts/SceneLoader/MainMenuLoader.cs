using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLoader : MonoBehaviour
{
    SceneLoader sceneLoader;

    void Start() => sceneLoader = FindObjectOfType<SceneLoader>();

    public void LoadMainMenu() => sceneLoader.LoadMainMenu();
}
