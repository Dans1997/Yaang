using System.Collections;
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

    public class Tile
    {
        bool isVisited = false;
        // Todo: Maybe add colors here?
    }

    [SerializeField] float lightTileTime;
    [Space]
    TilePath[] path;

    // State
    Vector3 startTilePos;
    Vector3 finishTilePos;
    Vector3 exitDoorPos; 

    // Cached Components
    PlayerController player;
    CameraFollow cameraFollow;
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        startTilePos = GameObject.FindGameObjectWithTag("Start").transform.position;
        finishTilePos = GameObject.FindGameObjectWithTag("Finish").transform.position;
        exitDoorPos = GameObject.FindGameObjectWithTag("Exit Door").transform.position;

        path = GetComponentsInChildren<TilePath>();
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
            player.transform.position = new Vector3(startTilePos.x, startTilePos.y, player.transform.position.z);
            cameraFollow.transform.position = new Vector3(startTilePos.x, startTilePos.y, cameraFollow.transform.position.z);
            cameraFollow.SetFollowObject(player.gameObject);
            path[0].LightUp(); // Ligth Up First Tile
            path[0].SetVisit(true);
        }
    }

    IEnumerator StartLevel()
    {
        player.enabled = false;
        yield return new WaitForSeconds(0.7f); // 70% of Transition Duration

        Camera mainCamera = cameraFollow.GetComponent<Camera>();
        Transform playerPos = player.transform;
        PlayerAnimation playerAnimation = player.GetComponentInChildren<PlayerAnimation>();

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

        IsTileValid(player.transform.position);
        yield return new WaitForSeconds(0.7f); // 70% of Tile Light Up Animation Duration

        // Light Up All Tile Paths
        foreach (TilePath tile in path)
        {
            StartCoroutine(cameraFollow.LerpFromTo(cameraFollow.transform.position, tile.transform.position, lightTileTime));
            yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y) == new Vector2(tile.transform.position.x, tile.transform.position.y));
            tile.LightUp();
            yield return new WaitForSeconds(0.7f); // 70% of Tile Light Up Animation Duration
        }

        // Turn Off All Tiles
        gameObject.BroadcastMessage("TurnOff");
        audioManager.PlaySound(AudioManager.SoundKey.TileLightDown1);
        yield return new WaitForSeconds(1.3f);

        // Show Exit Door
        StartCoroutine(cameraFollow.LerpFromTo(cameraFollow.transform.position, exitDoorPos, 2f));
        yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y) == new Vector2(exitDoorPos.x, exitDoorPos.y));
        yield return new WaitForSeconds(2f); // Time to show Exit Door

        // Go to Player Position
        cameraFollow.SetFollowObject(player.gameObject);
        cameraFollow.SetMoveSpeed(8f);
        yield return new WaitUntil(() => new Vector2(cameraFollow.transform.position.x, cameraFollow.transform.position.y) == new Vector2(player.transform.position.x, player.transform.position.y));

        cameraFollow.SetMoveSpeed(3f);
        player.enabled = true;
    }

    public bool IsTileValid(Vector3 position)
    {
        TilePath tile = System.Array.Find(path, t => t.transform.position == position);
        if(tile != null)
        {
            tile.LightUp();
            tile.SetVisit(true);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsDestination(Vector3 position) => new Vector2(position.x, position.y) == new Vector2(finishTilePos.x, finishTilePos.y);

}
