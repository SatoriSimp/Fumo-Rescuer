using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButtons : MonoBehaviour
{
    public Button[] LevelButtons;

    private int NumberOfCompletedLevels = 0;

    private void Start()
    {
        NumberOfCompletedLevels = PlayerPrefs.GetInt("HighestLevelCompleted");

        for (int i = 0; i < LevelButtons.Length; i++)
        {
            if (i <= NumberOfCompletedLevels) continue;
            LevelButtons[i].enabled = false;
            LevelButtons[i].interactable = false;
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OnClickLoadLevel(int LevelIndex)
    {
        SceneManager.LoadScene(LevelIndex + 3);
        GameObject.FindFirstObjectByType<AudioScript>().StopMusic();
    }
}
