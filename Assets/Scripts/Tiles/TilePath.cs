using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TilePath : MonoBehaviour
{
    // State
    protected bool wasVisited = false;

    public abstract void OnVisit(PlayerController player);

    public abstract void PlayTurnOnSound();

    public void ResetTile()
    {
        if (tag.Contains("Start"))
        {
            wasVisited = true;
            return;
        }
        wasVisited = false;
        TurnOff();
    }

    public virtual bool WasVisited() => wasVisited;

    public void SetVisit(bool visited) => wasVisited = visited;

    public void TurnOn() => GetComponent<Animator>().SetTrigger("lightUpTrigger");

    public void TurnOff() => GetComponent<Animator>().SetTrigger("lightDownTrigger");

    public IEnumerator LightUpTemporarily(float duration)
    {
        TurnOn();
        yield return new WaitForSeconds(duration);
        TurnOff();
    }
}
