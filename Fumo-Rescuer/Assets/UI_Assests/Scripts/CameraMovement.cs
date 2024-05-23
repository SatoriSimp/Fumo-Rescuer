using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera Camera;
    public Transform player;
    public Canvas canvas;
    public WinningScript winningScript;

    public GameObject PlayerStatusBar;

    public float moveTime;
    public Vector3 offset;
    public int size;

    public float sizeStayTime;
    private float sizeStayCountdown = 0;

    public float positionStayTime;
    private float positionStayCountdown = 0;

    public bool haveBriefLevelShowcase = false;

    private Vector3 InitialPlayerPosition;
    private bool isPlayerFirstMovementConfirmed = false;

    // Start is called before the first frame update
    void Start()
    {
        InitialPlayerPosition = player.position;
        PlayerStatusBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if (haveBriefLevelShowcase)
        {
            positionStayCountdown += Time.deltaTime;
            if (positionStayCountdown >= positionStayTime)
            {
                isPlayerFirstMovementConfirmed = true;
                haveBriefLevelShowcase = false;
            }
        }

        if (!isPlayerFirstMovementConfirmed)
        {
            isPlayerFirstMovementConfirmed = (player.position != InitialPlayerPosition);
            return;
        }

        sizeStayCountdown += Time.deltaTime;
        if (sizeStayCountdown >= sizeStayTime)
        {
            Camera.orthographicSize = size;
            PlayerStatusBar.SetActive(true);
        }
        Vector3 finalPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, finalPosition, moveTime * Time.deltaTime);
    }

    public IEnumerator SwitchLockingIntoNewPlayer(PlayerBehaviorScript newPlayer)
    {
        player = newPlayer.transform;

        winningScript.preventUpdate = true;
        winningScript.player = newPlayer;

        GameButtonsScript[] GameButtons = canvas.GetComponentsInChildren<GameButtonsScript>();
        foreach (GameButtonsScript button in GameButtons)
        {
            button.alwaysOnDisplay = true;
            button.player = newPlayer;
        }

        // Wait for next frame to ensure new player is fully instantiated and assigned
        yield return new WaitForEndOfFrame();

        winningScript.preventUpdate = false;
        foreach (GameButtonsScript button in GameButtons)
        {
            button.alwaysOnDisplay = false;
        }
    }
}
