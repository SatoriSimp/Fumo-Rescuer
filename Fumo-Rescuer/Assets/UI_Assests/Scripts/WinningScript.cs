using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinningScript : MonoBehaviour
{
    public PlayerBehaviorScript player;
    public CanvasGroup canvasGroup;
    public TMP_Text textMeshPro;

    private bool preventUpdate = false;

    // Start is called before the first frame update
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        textMeshPro = gameObject.GetComponent<TMP_Text>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (preventUpdate) return;

        if (!player || player.currentHealth <= 0)
        {
            preventUpdate = true;
            textMeshPro.text = "You Lost!";
            textMeshPro.color = new Color(255, 0, 0, 0.8f);
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    // Update is called once per frame
    public void PlayerVictoryConfirmed()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("GameButtons");
        foreach (GameObject button in buttons)
        {
            GameButtonsScript Retry = button.GetComponent<GameButtonsScript>();
            Retry.EnableButton();
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyBehaviorScript>().TakeDamage(0, 0, 99999, player);
        }
    }
}
