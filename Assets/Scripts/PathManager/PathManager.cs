﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathManager : MonoBehaviour
{
    #region Singleton

    private static PathManager _instance;

    public static PathManager PathManagerInstance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Path Manager is null!");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    #endregion Singleton

    [SerializeField] float lightTileTime;
    [Space]
    [SerializeField] TilePath[] path;

    // State
    Vector3 start;
    Vector3 finish;
    PlayerController player;
    CameraFollow cameraFollow;
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        start = GameObject.FindGameObjectWithTag("Start").transform.position;
        finish = GameObject.FindGameObjectWithTag("Finish").transform.position;
        player = FindObjectOfType<PlayerController>();
        cameraFollow = FindObjectOfType<CameraFollow>();
        audioManager = AudioManager.AudioManagerInstance;

        GameManager gameManager = GameManager.GameManagerInstance;
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;

        if (gameManager.IsFirstVisit(currentBuildIndex))
        {
            gameManager.SetFirstVisit(currentBuildIndex, false);
            StartCoroutine(StartLevel());
        }
        else
        {
            cameraFollow.SetFollowObject(player.gameObject);
        }
    }

    IEnumerator StartLevel()
    {
        player.enabled = false;
        yield return new WaitForSeconds(0.7f); // 70% of Transition Duration

        Vector3 playerDestination = new Vector3(start.x, start.y -1, start.z); // 1 unit before first tile
        Camera mainCamera = cameraFollow.GetComponent<Camera>();
        Transform playerPos = player.transform;

        float previousOrthoSize = mainCamera.orthographicSize;
        mainCamera.orthographicSize = 4f;

        // Move Player to 1 unit before first tile path
        for (float t = 0f; t < 3f; t += Time.deltaTime)
        {
            playerPos.position = Vector3.MoveTowards(playerPos.position, playerDestination, Time.deltaTime * 2);
            if (playerPos.position == playerDestination) break;
            audioManager.PlaySound(AudioManager.SoundKey.Footstep);
            yield return 0;
        }
        playerPos.position = playerDestination;

        // Zoom camera in
        while(mainCamera.orthographicSize > previousOrthoSize)
        {
            mainCamera.orthographicSize -= Time.deltaTime * 2;
            yield return 0;
        }
        mainCamera.orthographicSize = previousOrthoSize;

        // Light Up All Tile Paths
        foreach (TilePath tile in path)
        {
            StartCoroutine(cameraFollow.LerpFromTo(cameraFollow.transform.position, tile.transform.position, lightTileTime));
            yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y) == new Vector2(tile.transform.position.x, tile.transform.position.y));
            tile.LightUp();
            yield return new WaitForSeconds(1f); // Tile Light Up Animation Duration
        }

        gameObject.BroadcastMessage("TurnOff");
        audioManager.PlaySound(AudioManager.SoundKey.TileLightDown1);
        yield return new WaitForSeconds(1.3f);

        StartCoroutine(cameraFollow.LerpFromTo(cameraFollow.transform.position, player.transform.position, lightTileTime));
        yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y) == new Vector2(player.transform.position.x, player.transform.position.y));

        cameraFollow.SetFollowObject(player.gameObject);
        player.enabled = true;
        IsTileValid(player.transform.position);
    }

    public bool IsTileValid(Vector3 position)
    {
        TilePath tile = System.Array.Find(path, t => t.transform.position == position);
        if(tile != null)
        {
            tile.LightUp();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsDestination(Vector3 position) => new Vector2(position.x, position.y) == new Vector2(finish.x, finish.y);

}
