using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int moveUnit = 2;
    [SerializeField] float transitionTime = 2f;

    // State
    bool isTrasitioning = false;
    Vector3 startPos;

    // To use own deltaTime;
    float myDeltaTime = 0f;
    float lastFrameTime = 0f;

    // Cached Components
    GameManager gameManager;

    public void Restart()
    {
        Debug.Log("Player Restarting!");
        transform.position = startPos;
        isTrasitioning = false;
        myDeltaTime = 0f;
        lastFrameTime = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        isTrasitioning = false;
        myDeltaTime = 0f;
        lastFrameTime = 0f;

        gameManager = GameManager.GameManagerInstance;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTrasitioning) return;

        int deltaX = 0;
        int deltaY = 0;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            deltaY = moveUnit;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            deltaX = moveUnit;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            deltaX = -moveUnit;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            deltaY = -moveUnit;
        }
        else return;

        Vector3 destination = new Vector3(transform.position.x + deltaX, transform.position.y + deltaY, 0);

        StartCoroutine(LerpFromTo(transform.position, destination, transitionTime));
    }

    private void LateUpdate()
    {
        myDeltaTime = Time.realtimeSinceStartup - lastFrameTime;
        lastFrameTime = Time.realtimeSinceStartup;
    }

    IEnumerator LerpFromTo(Vector3 pos1, Vector3 pos2, float duration)
    {
        isTrasitioning = true;
        Time.timeScale = 0f;
        for (float t = 0f; t < duration; t += myDeltaTime)
        {
            transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            yield return 0;
        }
        isTrasitioning = false;
        Time.timeScale = 1f;
        transform.position = pos2;

        // Check if tile is valid or not
        if(gameManager.IsTileValid(transform.position))
        {
            if (gameManager.IsDestination(transform.position))
            {
                Debug.Log("I'm ready for the next level");
            }
        }
        else
        {
            // Kill Player
            Restart();
        }
    }
}
