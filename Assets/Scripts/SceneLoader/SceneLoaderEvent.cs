using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderEvent : MonoBehaviour
{
    public SceneLoader sceneLoader;

    public void LoadNextScene(float duration) => StartCoroutine(CallLoader(duration));

    private IEnumerator CallLoader(float duration)
    {
        yield return new WaitForSeconds(duration);
        sceneLoader.LoadNextScene();
    }
}
