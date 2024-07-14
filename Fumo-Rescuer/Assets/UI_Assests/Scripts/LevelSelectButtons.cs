using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButtons : MonoBehaviour
{
    public Button[] LevelButtons;

    public Button NextButton, Level_6_2;
    public Sprite Swap_Icon_Default, Swap_Icon_Pressed;

    private bool isNextPage = false;

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

        Level_6_2.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(NumberOfCompletedLevels >= 6);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Swap()
    { 
        isNextPage = !isNextPage;

        for (int i = 0; i < LevelButtons.Length; i++)
        {
            LevelButtons[i].gameObject.SetActive(!isNextPage);
        }
        Level_6_2.gameObject.SetActive(isNextPage);

        NextButton.GetComponent<Image>().sprite = isNextPage ? Swap_Icon_Pressed : Swap_Icon_Default;
    }

    public void OnClickLoadLevel(int LevelIndex)
    {
        SceneManager.LoadScene(LevelIndex + 4);
        GameObject.FindFirstObjectByType<AudioScript>().StopMusic();
    }
}
