using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CyberTile : TilePath
{
    // Fire Tile -> I shouldn't need to say that, but be careful with the deadly zap-zaps.

    // State
    bool zapped = false;
    float zapCooldown = 2f;
    [SerializeField] [Range(0f, 10f)] float zapDelay = 0f;

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
        yield return new WaitForSeconds(zapDelay);
        while (true)
        {
            yield return new WaitForSeconds(zapCooldown);
            zapTileVFX.Play();
            AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.CyberTileZap1, transform.position);
            yield return new WaitForSeconds(mainModule.duration);
            zapTileVFX.Stop();
        }
    }

    public override void PlayTurnOnSound()
    {
        //AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.CyberTile1);
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
