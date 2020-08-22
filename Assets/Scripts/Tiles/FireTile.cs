using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FireTile : TilePath
{
    // Fire Tile -> Ouch, so much for cold feet.

    // Cached Components
    ParticleSystem fireTileVFX;

    private void Start()
    {
        fireTileVFX = GetComponent<ParticleSystem>();
    }

    public override void OnVisit(PlayerController player)
    {
        Debug.Log("You just got scorched.");
        fireTileVFX.Play();   
        AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.FireTileLightUp1, player.transform.position);
        StartCoroutine(player.KillPlayer());
    }

    public override bool WasVisited()
    {
        return true;
    }

    public override void PlayTurnOnSound()
    {
        // Change This 
        int randomSound = Random.Range((int)AudioManager.SoundKey.TileLightUp1, (int)AudioManager.SoundKey.TileLightUp4);
        AudioManager.AudioManagerInstance.PlaySound((AudioManager.SoundKey)randomSound);
    }
}
