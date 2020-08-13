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

    // Cached Components
    GameManager gameManager;
    PlayerAnimation playerAnimation;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        isTrasitioning = false;

        gameManager = GameManager.GameManagerInstance;
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
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

    IEnumerator LerpFromTo(Vector3 pos1, Vector3 pos2, float duration)
    {
        isTrasitioning = true;
        playerAnimation.SetMove();


        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            yield return 0;
        }
        isTrasitioning = false;

        transform.position = pos2;
        CheckPlayerPosition();
    }

    private void CheckPlayerPosition()
    {
        // Check if tile is valid or not
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
        float length = playerAnimation.GetAnimationLength("Death");
        Debug.Log(length);
        yield return new WaitForSeconds(playerAnimation.GetAnimationLength("Death"));

        playerAnimation.PlayAnimation("Idle");
        transform.position = startPos;
        isTrasitioning = false;
    }
}
