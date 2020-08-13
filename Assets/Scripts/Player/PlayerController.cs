using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int moveUnit = 2;
    [SerializeField] float transitionTime = 2f;

    // State
    bool isIdle = true;
    Vector3 startPos;

    // Cached Components
    GameManager gameManager;
    PlayerAnimation playerAnimation;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        isIdle = true;

        gameManager = GameManager.GameManagerInstance;
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        playerAnimation.SetIdle(isIdle);
        if (!isIdle) return;

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

        playerAnimation.SetMoveDirection(new Vector2(deltaX, deltaY));

        Vector3 destination = new Vector3(transform.position.x + deltaX, transform.position.y + deltaY, 0);

        StartCoroutine(LerpFromTo(transform.position, destination, transitionTime));
    }

    IEnumerator LerpFromTo(Vector3 pos1, Vector3 pos2, float duration)
    {
        isIdle = false;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            yield return 0;
        }
        isIdle = true;
        transform.position = pos2;

        // Check if the tile is valid or not
        CheckPlayerPosition();
    }

    private void CheckPlayerPosition()
    {
        if (gameManager.IsTileValid(transform.position))
        {
            if (gameManager.IsDestination(transform.position))
            {
                Debug.Log("I'm ready for the next level");
            }
        }
        else
        {
            StartCoroutine(KillPlayer());
        }
    }

    IEnumerator KillPlayer()
    {
        playerAnimation.SetDeath();
        yield return new WaitForSeconds(1f);

        playerAnimation.PlayAnimation("Idle");
        transform.position = startPos;
        isIdle = true;
    }
}
