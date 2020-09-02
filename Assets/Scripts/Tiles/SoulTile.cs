﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SoulTile : TilePath
{
    // Soul Tile -> This one doesn't hurt you, but it will get you lost.

    // State
    [SerializeField] bool canBounce = false; // If enabled, the player will bounce between Soul Tiles

    // Cached Components
    [SerializeField] TilePath pairTile = null;
    SoulTile soulTile = null;
    Animator soulTileAnimator;
    AudioManager audioManager;
    CameraFollow cameraFollow;
    PathManager pathManager;

    private void Start()
    {
        if (pairTile == null) Debug.LogWarning("Soul Tile has no pair to teleport to!");
        soulTileAnimator = GetComponent<Animator>();
        audioManager = AudioManager.AudioManagerInstance;
        cameraFollow = FindObjectOfType<CameraFollow>();
        pathManager = PathManager.PathManagerInstance;

        if (pairTile.GetType() == typeof(SoulTile))
        {
            soulTile = pairTile.GetComponent<SoulTile>();
        }
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
        
        if(soulTile)
        {
            soulTile.soulTileAnimator.Play("Soul_Teleport_Out", 0);
        }

        player.enabled = true;
        player.transform.position = pairTile.transform.position;
        if (cameraFollow.IsCameraMovementEnabled()) cameraFollow.transform.position = player.transform.position;

        if (!pairTile.WasVisited())
        {
            if (soulTile)
            {
                if (soulTile.canBounce)
                {
                    player.enabled = false;
                    yield return new WaitForSeconds(1f);
                    pathManager.CheckPlayerPosition();
                } 
                else
                {
                    pairTile.TurnOn();
                    pairTile.SetVisit(true);
                    pathManager.IncrementVisitedCount();
                }
            }
            else
            {
                pathManager.CheckPlayerPosition();
            }
        }
    }

    public override void PlayTurnOnSound()
    {
        //TODO: Different Turn On Sound for the Soul Tile
        int randomSound = Random.Range((int)AudioManager.SoundKey.TileLightUp1, (int)AudioManager.SoundKey.TileLightUp4);
        AudioManager.AudioManagerInstance.PlaySound((AudioManager.SoundKey)randomSound);
    }

    private void OnDrawGizmos()
    {
        if (!pairTile) return;
        Gizmos.color = Color.green;

        Vector3 pos = transform.position;
        Vector3 direction = (pairTile.transform.position - pos)/2;
        float arrowHeadAngle = 30f;
        float arrowHeadLength = 0.25f;

        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }
}
