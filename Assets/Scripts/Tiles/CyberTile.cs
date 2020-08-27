using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CyberTile : TilePath
{
    // Fire Tile -> I shouldn't need to say that, but be careful with the deadly zap-zaps.

    // State
    bool zapped = false;
    float zapDelay = 2f;

    // Cached Components
    PlayerController playerController;
    ParticleSystem zapTileVFX;
    ParticleSystem.MainModule mainModule;

    private void Start()
    {
        zapTileVFX = GetComponent<ParticleSystem>();
        mainModule = zapTileVFX.main;
        if (zapTileVFX) mainModule.simulationSpeed = 1f;
        StartCoroutine(ZapLoop());
    }

    public override string GetName() => "Cyber Tile";

    public override bool WasVisited() => true;

    public override void OnVisit(PlayerController player)
    {
        bool previousWasVisited = wasVisited;
        wasVisited = true;
       
        if (!previousWasVisited)
        {
            TurnOn();
            playerController = player;
        }
    }

    IEnumerator ZapLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(zapDelay);
            zapTileVFX.Play();
            AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.CyberTileZap1, transform.position);
            yield return new WaitForSeconds(mainModule.duration);
            zapTileVFX.Stop();
        }
    }

    public override void PlayTurnOnSound()
    {
        AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.CyberTile1);
    }

    private void Update()
    {
        if (!playerController || transform.position != playerController.transform.position) return;
        if (zapTileVFX.isEmitting && !zapped)
        {
            zapped = true;
            Debug.Log("You just got zapped.");
            StartCoroutine(playerController.KillPlayer());
        }
    }
}
