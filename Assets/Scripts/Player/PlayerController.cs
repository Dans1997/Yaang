﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] float transitionTime = 2f;

    [Space]
    [Header("Player Power")]
    [SerializeField] bool hasUsedPower = false;

    float moveUnitY = 1f;
    float moveUnitX = 1.25f;

    // State
    bool isIdle = true;

    // Cached Components
    PathManager pathManager;
    PlayerAnimation playerAnimation;
    AudioManager audioManager;
    Animator playerPower;
    Animator playerPowerHUD;
    ParticleSystem powerParticles;

    public Vector2 GetMoveUnits() => new Vector2(moveUnitX, moveUnitY);

    public void SetWinState()
    {
        playerAnimation.SetMoveDirection(new Vector2(0, 0));
        playerAnimation.SetIdle(true);
        this.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        isIdle = true;

        pathManager = PathManager.PathManagerInstance;
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
        audioManager = AudioManager.AudioManagerInstance;
        playerPower = GameObject.FindGameObjectWithTag("PlayerPower").GetComponent<Animator>();
        playerPowerHUD = GameObject.FindGameObjectWithTag("PlayerPowerHUD").GetComponent<Animator>();
        powerParticles = playerPower.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        playerAnimation.SetIdle(isIdle);
        if (!isIdle) return;

        HandlePowerInput();
        HandleMovementInput();
    }

    private void HandlePowerInput()
    {
        if (hasUsedPower) return;
        if(Input.GetKeyDown(KeyCode.Space) || CrossPlatformInputManager.GetButtonDown("PowerButton"))
        {
            hasUsedPower = true;
            playerPower.SetTrigger("activateTrigger");
            playerPowerHUD.SetTrigger("activateTrigger");
            powerParticles.Play();
            audioManager.PlaySound(AudioManager.SoundKey.PlayerPower);
        }
    }

    private void HandleMovementInput()
    {
        float moveX = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        float moveY = CrossPlatformInputManager.GetAxisRaw("Vertical");
        bool downPressed = CrossPlatformInputManager.GetButtonDown("Down");
        bool downRightPressed = CrossPlatformInputManager.GetButtonDown("Down_Right");
        bool rightPressed = CrossPlatformInputManager.GetButtonDown("Right");
        bool rightUpPressed = CrossPlatformInputManager.GetButtonDown("Right_Up");
        bool upPressed = CrossPlatformInputManager.GetButtonDown("Up");
        bool upLeftPressed = CrossPlatformInputManager.GetButtonDown("Up_Left");
        bool leftPressed = CrossPlatformInputManager.GetButtonDown("Left");
        bool leftDownPressed = CrossPlatformInputManager.GetButtonDown("Left_Down");

        bool pressedAnyButton = downPressed || upPressed || rightPressed || leftPressed
            || downRightPressed || rightUpPressed || upLeftPressed || leftDownPressed;

        bool usedXAxis = Mathf.Abs(moveX) >= 0.5f;
        bool usedYAxis = Mathf.Abs(moveY) >= 0.5f;
        bool usedJoystick = usedXAxis || usedYAxis;

        if (!usedJoystick && !pressedAnyButton)
        {
            playerAnimation.SetIdle(true);
            return;
        }

        if (usedXAxis) moveX = Mathf.Sign(moveX) * 1f;
        if (usedYAxis) moveY = Mathf.Sign(moveY) * 1f;

        // DOWN
        if (downPressed)
        {
            moveX = 0f;
            moveY = -1f;
        }

        // DOWN_RIGHT
        if (downRightPressed)
        {
            moveX = 1f;
            moveY = -1f;
        }

        // RIGHT
        if (rightPressed)
        {
            moveX = 1f;
            moveY = 0f;
        }

        // RIGHT_UP
        if (rightUpPressed)
        {
            moveX = 1f;
            moveY = 1f;
        }

        // UP
        if (upPressed)
        {
            moveX = 0f;
            moveY = 1f;
        }

        // UP_LEFT
        if (upLeftPressed)
        {
            moveX = -1f;
            moveY = 1f;
        }

        // LEFT
        if (leftPressed)
        {
            moveX = -1f;
            moveY = 0f;
        }

        // LEFT_DOWN
        if (leftDownPressed)
        {
            moveX = -1f;
            moveY = -1f;
        }

        MovePlayer(moveX, moveY);
    }

    private void MovePlayer(float moveX, float moveY)
    {
        audioManager.PlaySound(AudioManager.SoundKey.PlayerMove);
        playerAnimation.SetMoveDirection(new Vector2(moveX, moveY));
        Vector3 destination = new Vector3(transform.position.x + moveX * moveUnitX, transform.position.y + moveY * moveUnitY, transform.position.z);
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
        transform.position = pos2;

        // Check if the tile is valid or not
        PathManager.PathManagerInstance.CheckPlayerPosition();
        isIdle = true;
    }

    public IEnumerator KillPlayer()
    {
        audioManager.PlaySound(AudioManager.SoundKey.PlayerDeath);
        playerAnimation.SetDeath();
        this.enabled = false;
        yield return new WaitForSeconds(1f);

        //StartCoroutine(cameraShake.ShakeCamera(.25f, 1f));
        //audioManager.PlaySound(AudioManager.SoundKey.PlayerGroundHit);
       // yield return new WaitForSeconds(0.5f);

        GameManager.GameManagerInstance.FailLevel();
    }
}
