using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Requires All Tiles to Win?")]
    [SerializeField] bool requiresAllTiles = false;

    [Space]
    [Header("Default View Camera")]
    [SerializeField] float lightTileTime = 0f;

    [Space]
    [Header("Overall View Camera")]
    [SerializeField] bool overallView = false;
    [SerializeField] float overallCameraTime = 11f;
    [SerializeField] float overallCameraTileTime = 0.25f;

    [Space]
    [Header("Static Camera")]
    [SerializeField] bool staticView = false;
    [SerializeField] Vector2 cameraCenter = new Vector2(0, 0);
    [SerializeField] float staticTileTime = 0.25f;

    // State
    Vector3 startTilePos;
    Vector3 finishTilePos;
    Vector3 exitDoorPos;
    int visitedTiles = 1;
    readonly float targetOrthoSize = 10f; // Covers all tiles from left to right

    // Cached Components
    PlayerController player;
    Canvas playerHUD;
    Text tileCountText;
    CameraFollow cameraFollow;
    AudioManager audioManager;
    TilePath[] path;

    // Start is called before the first frame update
    void Start()
    {
        startTilePos = GameObject.FindGameObjectWithTag("Start").transform.position;
        finishTilePos = GameObject.FindGameObjectWithTag("Finish").transform.position;
        exitDoorPos = GameObject.FindGameObjectWithTag("Exit Door").transform.position;

        path = GetComponentsInChildren<TilePath>();
        player = FindObjectOfType<PlayerController>();
        playerHUD = GameObject.FindGameObjectWithTag("Player HUD").GetComponent<Canvas>();
        tileCountText = GameObject.FindGameObjectWithTag("TileCountUI").GetComponent<Text>();
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
            player.transform.position = new Vector3(startTilePos.x, startTilePos.y, player.transform.position.z);
            cameraFollow.transform.position = new Vector3(startTilePos.x, startTilePos.y, cameraFollow.transform.position.z);
            cameraFollow.SetFollowObject(player.gameObject);
            path[0].TurnOn(); // Light Up First Tile
            path[0].SetVisit(true);

            // If Static View Is Enabled, Ensure Camera is in Given Position
            if (staticView)
            { 
                cameraFollow.GetComponent<Camera>().orthographicSize = targetOrthoSize;
                cameraFollow.transform.position = new Vector3(cameraCenter.x, cameraCenter.y, cameraFollow.transform.position.z);
                cameraFollow.EnableCameraMovement(false);
            }
        }

        // Update Tile Count Text
        tileCountText.text = $"{visitedTiles}/{path.Length}";
    }

    IEnumerator StartLevel()
    {
        player.enabled = false;
        playerHUD.enabled = false;
        yield return new WaitForSeconds(0.7f); // 70% of Transition Duration

        // Components Needed For the Start Cutscene
        Camera mainCamera = cameraFollow.GetComponent<Camera>();
        Transform playerPos = player.transform;
        PlayerAnimation playerAnimation = player.GetComponentInChildren<PlayerAnimation>();

        // Camera Zoom Related Variables
        float startCameraSize = mainCamera.orthographicSize;

        // Static View Variables
        Vector3 centerPos = new Vector3(cameraCenter.x, cameraCenter.y, cameraFollow.transform.position.z);

        // Move Player to First Tile Path
        playerAnimation.PlayAnimation("Move_Up_Loop");
        for (float t = 0f; t < 3f; t += Time.deltaTime)
        {
            playerPos.position = Vector3.MoveTowards(playerPos.position, startTilePos, Time.deltaTime * 2);
            if (playerPos.position == startTilePos) break;
            audioManager.PlaySound(AudioManager.SoundKey.Footstep);
            yield return 0;
        }
        playerAnimation.PlayAnimation("Idle");
        playerPos.position = startTilePos;

        yield return new WaitForSeconds(0.7f); // 70% of Tile Light Up Animation Duration

        if (staticView)
        {
            // Move Camera to The Given Center and Zoom Camera Out 
            StartCoroutine(cameraFollow.LerpFromTo(cameraFollow.transform.position, centerPos, 2f));
            while (mainCamera.orthographicSize < targetOrthoSize)
            {
                mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, targetOrthoSize, Time.deltaTime * 3);
                yield return 0;
            }
            yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y) 
            == new Vector2(centerPos.x, centerPos.y));
            mainCamera.orthographicSize = targetOrthoSize;

            // Light Up All Tiles
            foreach (TilePath tile in path)
            {
                tile.TurnOn();
                tile.PlayTurnOnSound();
                yield return new WaitForSeconds(staticTileTime);
            }
            yield return new WaitForSeconds(3f);

            // Turn Off All Tiles
            gameObject.BroadcastMessage("ResetTile");
            audioManager.PlaySound(AudioManager.SoundKey.TileLightDown1);
            yield return new WaitForSeconds(1.3f);

            // Enable Player Control - But Don't Set Camera Follow
            player.enabled = true;
            playerHUD.enabled = true;
            cameraFollow.EnableCameraMovement(false);
            yield break;
        }

        if (overallView && !staticView)
        {
            // Zoom Camera Out          
            while (mainCamera.orthographicSize < targetOrthoSize)
            {
                mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, targetOrthoSize, Time.deltaTime);
                yield return 0;
            }
            mainCamera.orthographicSize = targetOrthoSize;

            // Move Camera to The Exit Door While Lighting Up Tiles
            StartCoroutine(cameraFollow.LerpFromTo(cameraFollow.transform.position, exitDoorPos, overallCameraTime));
            foreach (TilePath tile in path)
            {        
                tile.TurnOn();
                tile.PlayTurnOnSound();
                yield return new WaitForSeconds(overallCameraTileTime);
            }
            yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y)
            == new Vector2(exitDoorPos.x, exitDoorPos.y));
        }
        
        if(!overallView && !staticView)
        {
            // Light Up All Tile Paths
            foreach (TilePath tile in path)
            {
                StartCoroutine(cameraFollow.LerpFromTo(cameraFollow.transform.position, tile.transform.position, lightTileTime));
                yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y) 
                == new Vector2(tile.transform.position.x, tile.transform.position.y));
                tile.TurnOn();
                tile.PlayTurnOnSound();
                yield return new WaitForSeconds(0.7f); // 70% of Tile Light Up Animation Duration
            }
        }

        // Turn Off All Tiles
        gameObject.BroadcastMessage("ResetTile");
        audioManager.PlaySound(AudioManager.SoundKey.TileLightDown1);
        yield return new WaitForSeconds(1.3f);

        // Show Exit Door
        StartCoroutine(cameraFollow.LerpFromTo(cameraFollow.transform.position, exitDoorPos, 2f));

        // Zoom Camera In If Necessary
        while (mainCamera.orthographicSize != startCameraSize)
        {
            mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, startCameraSize, Time.deltaTime);
            yield return 0;
        }
        mainCamera.orthographicSize = startCameraSize;

        yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y)
        == new Vector2(exitDoorPos.x, exitDoorPos.y));
        yield return new WaitForSeconds(2f); // Time to show Exit Door

        // Setup Camera to Go to Player Position
        cameraFollow.SetMoveSpeed(12f);
        cameraFollow.SetFollowObject(player.gameObject);
        yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y)
        == new Vector2(player.transform.position.x, player.transform.position.y));

        // Enable Player Control
        cameraFollow.SetMoveSpeed(3f);
        player.enabled = true;
        playerHUD.enabled = true;
    }

    public void CheckPlayerPosition()
    {
        Vector3 position = player.transform.position;
        TilePath tile = System.Array.Find(path, t => t.transform.position == position);
        if (tile != null)
        {
            if (!tile.WasVisited()) IncrementVisitedCount();
            tile.OnVisit(player);
            if (new Vector2(position.x, position.y) == new Vector2(finishTilePos.x, finishTilePos.y))
            {
                if (AreAllTilesVisited())
                {
                    player.FacePlayerUp();
                    GameManager.GameManagerInstance.CompleteLevel();
                }
            }
        }
        else
        {
            // No tile found! Kill player and restart level
            StartCoroutine(player.KillPlayer());
        }
    }

    public void IncrementVisitedCount() => tileCountText.text = $"{++visitedTiles}/{path.Length}";

    public bool AreAllTilesVisited()
    {
        if (!requiresAllTiles) return true;
        if (visitedTiles > path.Length) Debug.LogWarning("Error in path count logic.");
        return visitedTiles == path.Length;
    }
}
