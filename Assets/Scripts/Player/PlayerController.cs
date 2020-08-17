using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Vector3 startPos;

    // Cached Components
    PathManager pathManager;
    PlayerAnimation playerAnimation;
    AudioManager audioManager;
    CameraShake cameraShake;
    Animator playerPower;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        isIdle = true;

        pathManager = PathManager.PathManagerInstance;
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
        audioManager = AudioManager.AudioManagerInstance;
        cameraShake = FindObjectOfType<CameraShake>();
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
        //if (hasUsedPower) return;
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            hasUsedPower = true;
            playerPower.SetTrigger("activateTrigger");
            audioManager.PlaySound(AudioManager.SoundKey.PlayerPower);
        }
    }

    private void HandleMovementInput()
    {
        float deltaX = 0;
        float deltaY = 0;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            deltaY = moveUnitY;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            deltaX = moveUnitX;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            deltaX = -moveUnitX;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            deltaY = -moveUnitY;
        }
        else return;

        playerAnimation.SetMoveDirection(new Vector2(deltaX, deltaY));
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
        isIdle = true;
        transform.position = pos2;

        // Check if the tile is valid or not
        CheckPlayerPosition();
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
        wantsToReboot = Input.GetKey(KeyCode.R);
        Debug.Log(previousWantsToReboot + " " + wantsToReboot);
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
