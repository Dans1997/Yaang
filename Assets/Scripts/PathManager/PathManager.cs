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
    Vector3 destination;
    PlayerController player;
    CameraFollow cameraFollow;

    // Start is called before the first frame update
    void Start()
    {
        destination = GameObject.FindGameObjectWithTag("Finish").transform.position;
        player = FindObjectOfType<PlayerController>();
        cameraFollow = FindObjectOfType<CameraFollow>();

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
        yield return new WaitForSeconds(1f); // Transition Duration


        foreach (TilePath tile in path)
        {
            StartCoroutine(cameraFollow.LerpFromTo(cameraFollow.transform.position, tile.transform.position, lightTileTime));
            yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y) == new Vector2(tile.transform.position.x, tile.transform.position.y));
            tile.LightUp();
            yield return new WaitForSeconds(1f); // Tile Light Up Animation Duration
        }

        gameObject.BroadcastMessage("TurnOff");
        StartCoroutine(cameraFollow.LerpFromTo(cameraFollow.transform.position, player.transform.position, lightTileTime));
        yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y) == new Vector2(player.transform.position.x, player.transform.position.y));

        player.enabled = true;
        cameraFollow.SetFollowObject(player.gameObject);
    }

    public bool IsTileValid(Vector3 position)
    {
        TilePath tile = System.Array.Find(path, t => t.transform.position == position);
        return tile != null;
    }

    public bool IsDestination(Vector3 position)
    {
        return position == destination;
    }

}
