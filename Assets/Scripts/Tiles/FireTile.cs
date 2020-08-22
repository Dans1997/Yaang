using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTile : TilePath
{
    // Fire Tile -> Ouch, so much for cold feet.

    public override void OnVisit(PlayerController player)
    {
        Debug.Log("You just got scorched.");
        StartCoroutine(player.KillPlayer());
    }

    public override void PlayTurnOnSound()
    {
        // Change This 
        int randomSound = Random.Range((int)AudioManager.SoundKey.TileLightUp1, (int)AudioManager.SoundKey.TileLightUp4);
        AudioManager.AudioManagerInstance.PlaySound((AudioManager.SoundKey)randomSound);
    }
}
