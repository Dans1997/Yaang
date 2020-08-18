using System.Collections;
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
    bool wantsToReboot = false;
    bool isIdle = true;

    // Cached Components
    PathManager pathManager;
    PlayerAnimation playerAnimation;
    AudioManager audioManager;
    Animator playerPower;

    // Start is called before the first frame update
    void Start()
    {
        isIdle = true;

        pathManager = PathManager.PathManagerInstance;
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
        audioManager = AudioManager.AudioManagerInstance;
        playerPower = GameObject.FindGameObjectWithTag("PlayerPower").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleReboot();
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
            audioManager.PlaySound(AudioManager.SoundKey.PlayerPower);
        }
    }

    private void HandleMovementInput()
    {
        float moveX = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        float moveY = CrossPlatformInputManager.GetAxisRaw("Vertical");
        bool downPressed = CrossPlatformInputManager.GetButtonDown("Down");
        bool upPressed = CrossPlatformInputManager.GetButtonDown("Up");
        bool rightPressed = CrossPlatformInputManager.GetButtonDown("Right");
        bool leftPressed = CrossPlatformInputManager.GetButtonDown("Left");

        bool pressedAnyButton = !downPressed || !upPressed || !rightPressed || !leftPressed;
        bool usedJoystick = Mathf.Abs(moveX) >= Mathf.Epsilon || Mathf.Abs(moveY) >= Mathf.Epsilon;

        if (!usedJoystick && !pressedAnyButton)
        {
            playerAnimation.SetIdle(isIdle);
            return;
        }

        // DOWN
        if ((moveX == 0 && moveY == -1) || downPressed)
        {
 
            playerAnimation.SetMoveDirection(new Vector2(0,-1));
            MovePlayer(0, -moveUnitY);
            return;
        }

        // RIGHT
        if ((moveX == 1 && moveY == 0) || rightPressed)
        {
            playerAnimation.SetMoveDirection(new Vector2(1, 0));
            MovePlayer(moveUnitX, 0);
            return;
        }

        // UP
        if ((moveX == 0 && moveY == 1) || upPressed)
        {
            playerAnimation.SetMoveDirection(new Vector2(0, 1));
            MovePlayer(0, moveUnitY);
            return;
        }

        // LEFT
        if ((moveX == -1 && moveY == 0) || leftPressed)
        {
            playerAnimation.SetMoveDirection(new Vector2(-1, 0));
            MovePlayer(-moveUnitX, 0);
            return;
        }
    }

    private void MovePlayer(float deltaX, float deltaY)
    {
        audioManager.PlaySound(AudioManager.SoundKey.PlayerMove);

        Vector3 destination = new Vector3(transform.position.x + deltaX, transform.position.y + deltaY, transform.position.z);
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
        CheckPlayerPosition();
        isIdle = true;
    }

    private void CheckPlayerPosition()
    {
        if (pathManager.IsTileValid(transform.position))
        {
            if (pathManager.IsDestination(transform.position))
            {
                playerAnimation.SetMoveDirection(new Vector2(0, 0));
                playerAnimation.SetIdle(true);
                this.enabled = false;
                GameManager.GameManagerInstance.CompleteLevel();
            }
        }
        else
        {
            StartCoroutine(KillPlayer());
        }
    }

    IEnumerator KillPlayer()
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

    private void HandleReboot()
    {
        bool previousWantsToReboot = wantsToReboot;
        wantsToReboot = Input.GetKey(KeyCode.R) || CrossPlatformInputManager.GetButton("Reboot");
        if (wantsToReboot && !previousWantsToReboot)
        {
            StartCoroutine(RebootLevel());
        }
    }

    IEnumerator RebootLevel()
    {
        Camera mainCamera = Camera.main;
        float currentOrthoSize = mainCamera.orthographicSize;

        AudioSource rebootAudioSource = AudioManager.AudioManagerInstance.PlaySound(AudioManager.SoundKey.Reboot, transform.position);
        for (float t = 0f; t < 3f; t += Time.deltaTime)
        {
            if (!wantsToReboot)
            {
                rebootAudioSource.Stop();
                break;
            }
            mainCamera.orthographicSize -= Time.deltaTime * 0.5f;
            yield return 0;
        }
        if (wantsToReboot) GameManager.GameManagerInstance.RebootLevel();
        else mainCamera.orthographicSize = currentOrthoSize;
    }
}
