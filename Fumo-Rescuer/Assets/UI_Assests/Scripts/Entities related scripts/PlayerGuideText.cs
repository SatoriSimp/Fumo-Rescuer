using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGuideText : MonoBehaviour
{
    public float timeUntilDisappear = 20.0f;
    public Transform player;
    private Vector2 PlayerInitPosition;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInitPosition = player.position;
        Destroy(gameObject, timeUntilDisappear);
    }

    private void Update()
    {
        if (player == null || !PlayerInitPosition.Equals(player.position))
            Destroy(gameObject);
    }
}
