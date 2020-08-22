using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTile : TilePath
{
    // Simple Tile -> When Stepped On, lights up and plays a random sound

    public override void OnVisit(PlayerController player)
    {
        if (wasVisited) return;
        wasVisited = true;
        TurnOn();
        PlayTurnOnSound();
    }

    public override void PlayTurnOnSound()
    {
        int randomSound = Random.Range((int)AudioManager.SoundKey.TileLightUp1, (int)AudioManager.SoundKey.TileLightUp4);
        AudioManager.AudioManagerInstance.PlaySound((AudioManager.SoundKey)randomSound);
    }
}
