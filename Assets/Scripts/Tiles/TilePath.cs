using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePath : MonoBehaviour
{
    // State
    bool wasVisited = false;

    public bool WasVisited() => wasVisited;

    public void SetVisit(bool visited) => wasVisited = visited;

    public void LightUp()
    {
        GetComponent<Animator>().SetTrigger("lightUpTrigger");
        if (wasVisited) return;
        int randomSound = Random.Range((int) AudioManager.SoundKey.TileLightUp1, (int) AudioManager.SoundKey.TileLightUp4);
        AudioManager.AudioManagerInstance.PlaySound((AudioManager.SoundKey) randomSound);
    }

    public void TurnOff()
    {
        GetComponent<Animator>().SetTrigger("lightDownTrigger");
    }

    public IEnumerator LightUpTemporarily()
    {
        LightUp();
        yield return 0;
        TurnOff();
    }
}
