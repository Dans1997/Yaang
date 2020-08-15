using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePath : MonoBehaviour
{
    // State
    bool wasVisited = false;

    // Cached Components
    Animator tilePathAnimator;

    private void Start()
    {
        tilePathAnimator = GetComponent<Animator>();
    }

    public void SetVisit(bool visited) => wasVisited = visited;

    public void LightUp()
    {
        tilePathAnimator.SetTrigger("lightUpTrigger");
        if (wasVisited) return;
        int randomSound = Random.Range((int) AudioManager.SoundKey.TileLightUp1, (int) AudioManager.SoundKey.TileLightUp4);
        AudioManager.AudioManagerInstance.PlaySound((AudioManager.SoundKey) randomSound);
    }

    public void TurnOff()
    {
        tilePathAnimator.SetTrigger("lightDownTrigger");
    }
}
