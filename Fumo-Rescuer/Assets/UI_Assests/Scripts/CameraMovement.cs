using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera Camera;
    public Transform player;
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
        if (sizeStayCountdown >= sizeStayTime) Camera.orthographicSize = size;

        Vector3 finalPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, finalPosition, moveTime * Time.deltaTime);
    }
}
