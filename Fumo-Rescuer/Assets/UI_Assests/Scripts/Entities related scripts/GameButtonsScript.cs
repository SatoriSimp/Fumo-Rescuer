using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButtonsScript : MonoBehaviour
{
    public PlayerBehaviorScript player;
    public CanvasGroup canvasGroup;
    public bool buttonTriggered = false;
    public bool isNextLevelButton = false;
    public bool alwaysOnDisplay = false;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (alwaysOnDisplay || buttonTriggered) return;

        if (player && player.currentHealth <= 0)
        {
            EnableButton();
        }
        else
        {
            // If the conditions are not met, hide the button
            canvasGroup.alpha = 0f; // This makes the button invisible
            canvasGroup.interactable = false; // This prevents the button from being clicked
            canvasGroup.blocksRaycasts = false; // This prevents the button from receiving mouse events
        }
    }

    public void EnableButton()
    {
        bool isFinalScene = SceneManager.sceneCountInBuildSettings == SceneManager.GetActiveScene().buildIndex + 1;
        // If the conditions are met, show the button
        canvasGroup.alpha = 1f; // This makes the button visible
        canvasGroup.interactable = !isNextLevelButton || (player.currentHealth > 0 && !isFinalScene); // This allows the button to be clicked
        canvasGroup.blocksRaycasts = true; // This allows the button to receive mouse events
        buttonTriggered = true;
    }

    public void LoadUserGuide()
    {
        SceneManager.LoadScene("PlayerGuide");
    }

    public void LoadCatalog()
    {
        SceneManager.LoadScene("Catalog");
    }


    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void ReplayGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToNextLevel()
    {
        int NextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(NextSceneIndex);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
