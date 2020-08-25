using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FireTile : TilePath
{
    // Fire Tile -> Ouch, so much for cold feet.

    // State
    float scorchDelay = 2f;

    // Cached Components
    ParticleSystem fireTileVFX;
    Coroutine scorchCoroutine;

    private void Start()
    {
        fireTileVFX = GetComponent<ParticleSystem>();
        if (fireTileVFX)
        {
            ParticleSystem.MainModule mainModule = fireTileVFX.main;
            mainModule.simulationSpeed = 1f;
        }
    }

    public override string GetName() => "Fire Tile";

    public override void OnVisit(PlayerController player)
    {
        bool previousWasVisited = wasVisited;
        wasVisited = true;

        if (!previousWasVisited)
        {
            TurnOn();
        }

        if (scorchCoroutine != null) StopCoroutine(scorchCoroutine);
        scorchCoroutine = StartCoroutine(ScorchPlayer(player));
    }

    IEnumerator ScorchPlayer(PlayerController player)
    {
        AudioSource fireTileWarmUp = AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.FireTileWarmUp, player.transform.position);
        for (float t = 0f; t < scorchDelay; t += Time.deltaTime)
        {
            if (transform.position != player.transform.position)
            {
                StopCoroutine(scorchCoroutine);
                fireTileWarmUp.Stop();
            }
            yield return 0;
        }
        if (transform.position == player.transform.position)
        {
            Debug.Log("You just got scorched.");
            fireTileVFX.Play();
            AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.FireTileLightUp1, player.transform.position);
            StartCoroutine(player.KillPlayer());
        }
    }

    public override void PlayTurnOnSound()
    {
        // Change This 
        int randomSound = Random.Range((int)AudioManager.SoundKey.TileLightUp1, (int)AudioManager.SoundKey.TileLightUp4);
        AudioManager.AudioManagerInstance.PlaySound((AudioManager.SoundKey)randomSound);
    }
}
