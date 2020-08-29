using System.Collections;
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

    private void Start()
    {
        if (pairTile && !pairTile.pairTile)
        {
            pairTile.pairTile = this;
        }
        soulTileAnimator = GetComponent<Animator>();
        audioManager = AudioManager.AudioManagerInstance;
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
        audioManager.PlaySound(AudioManager.SoundKey.Teleport1);
        audioManager.PlaySound(AudioManager.SoundKey.Teleport2);
        yield return new WaitForSeconds(1f);

        player.enabled = true;
        player.transform.position = pairTile.transform.position;
        Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, Camera.main.transform.position.z);

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
