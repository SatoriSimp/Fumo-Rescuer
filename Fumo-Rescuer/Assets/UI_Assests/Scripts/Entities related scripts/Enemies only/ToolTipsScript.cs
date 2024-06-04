using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipsScript : MonoBehaviour
{
    private readonly int desired_X = -330;
    private readonly int initial_X = 130;

    public int current_X;
    private int totalFrames = 30;
    private int offsetPerFrame;

    private RectTransform rectTransform;
    private Vector2 offset = new(130, -40);

    public float stayTime = 10.0f;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        offsetPerFrame =  Mathf.Abs(desired_X - initial_X) / totalFrames;
        current_X = initial_X;
        // Position the tooltip at the top right corner of the screen
        rectTransform.anchorMin = new Vector2(1, 1); // Top right corner
        rectTransform.anchorMax = new Vector2(1, 1); // Top right corner
        rectTransform.pivot = new Vector2(1, 1); // Pivot at the top right corner
        rectTransform.anchoredPosition = offset; // Offset from the top right corner
    }

    private void Update()
    {
        stayTime -= Time.deltaTime;
        if (stayTime > 0)
        {
            if (current_X > desired_X)
            {
                Entrance();
            }
            else current_X = desired_X;
        }
        else
        {
            ExitAndDestroy();
        }
    }

    private void Entrance()
    {
        current_X -= offsetPerFrame;
        rectTransform.anchoredPosition = new Vector2(current_X, -40);
    }

    private void ExitAndDestroy()
    {
        current_X += offsetPerFrame;
        rectTransform.anchoredPosition = new Vector2(current_X, -40);
        if (current_X >= initial_X + offsetPerFrame) Destroy(gameObject);
    }
}
