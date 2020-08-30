﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SoulTile : TilePath
{
    // Soul Tile -> This one doesn't hurt you, but it will get you lost.

    // Cached Components
    [SerializeField] SoulTile pairTile;
    Animator soulTileAnimator;
    AudioManager audioManager;
    Camera mainCamera;

    private void Start()
    {
        if (pairTile && !pairTile.pairTile)
        {
            pairTile.pairTile = this;
        }
        soulTileAnimator = GetComponent<Animator>();
        audioManager = AudioManager.AudioManagerInstance;
        mainCamera = Camera.main;
    }

    public override string GetName() => "Soul Tile";

    public override void OnVisit(PlayerController player)
    {
        bool previousWasVisited = wasVisited;
        wasVisited = true;

        if (!previousWasVisited)
        {
            TurnOn();
        }

        StartCoroutine(TeleportPlayer(player));
    }

    IEnumerator TeleportPlayer(PlayerController player)
    {
        player.SetIdle(true);
        player.enabled = false;
        soulTileAnimator.SetTrigger("teleportTrigger");
        audioManager.PlaySound(AudioManager.SoundKey.SoulTileStep);
        audioManager.PlaySound(AudioManager.SoundKey.TeleportIn);
        yield return new WaitForSeconds(1f);

        audioManager.PlaySound(AudioManager.SoundKey.TeleportOut);
        pairTile.soulTileAnimator.Play("Soul_Teleport_Out", 0);

        player.enabled = true;
        player.transform.position = pairTile.transform.position;
        mainCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, mainCamera.transform.position.z);

        if (!pairTile.WasVisited())
        {
            pairTile.TurnOn();
            pairTile.SetVisit(true);
            PathManager.PathManagerInstance.IncrementVisitedCount();
        }
    }

    public override void PlayTurnOnSound()
    {
        //TODO: Different Turn On Sound for the Soul Tile
        int randomSound = Random.Range((int)AudioManager.SoundKey.TileLightUp1, (int)AudioManager.SoundKey.TileLightUp4);
        AudioManager.AudioManagerInstance.PlaySound((AudioManager.SoundKey)randomSound);
    }
}
