using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePath : MonoBehaviour
{
    Animator tilePathAnimator;

    private void Start()
    {
        tilePathAnimator = GetComponent<Animator>();
    }

    public void LightUp()
    {
        //Debug.Log("LIGHT IT UP! " + gameObject.name);
        tilePathAnimator.SetTrigger("lightUpTrigger");
        int randomSound = Random.Range((int) AudioManager.SoundKey.TileLightUp1, (int) AudioManager.SoundKey.TileLightUp4);
        AudioManager.AudioManagerInstance.PlaySound((AudioManager.SoundKey) randomSound);
    }

    public void TurnOff()
    {
        //Debug.Log("LIGHT IT DOWN! " + gameObject.name);
        tilePathAnimator.SetTrigger("lightDownTrigger");
    }
}
