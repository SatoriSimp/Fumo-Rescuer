using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleScript : MonoBehaviour
{
    public GameObject EventSystem;
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision && collision.CompareTag("Player"))
        {
            EventSystem.GetComponent<Stage6_1_Script>().TriggerWinAndMoveToNextSection();
            triggered = true;
        }
    }
}
