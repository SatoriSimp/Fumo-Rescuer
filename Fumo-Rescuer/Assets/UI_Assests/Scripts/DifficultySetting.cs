using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySetting : MonoBehaviour
{
    [SerializeField] private Button AddDiff, LowerDiff;
    [SerializeField] private TMP_Text DifficultyText;

    private int gameDiff;
    private int playerProgress;

    // Start is called before the first frame update
    void Start()
    {
        gameDiff = PlayerPrefs.GetInt("Difficulty", 1);
        playerProgress = PlayerPrefs.GetInt("HighestLevelCompleted", 1);
        UpdateUI();
    }

    public void DifficultyChange(int value)
    {
        gameDiff += value;
        if (gameDiff < 0) gameDiff = 0;
        else if (gameDiff > 3) gameDiff = 3;

        PlayerPrefs.SetInt("Difficulty", gameDiff);
        UpdateUI();
    }

    public void UpdateUI()
    {
        Color textColor;
        string txt;

        switch (gameDiff)
        {
            case 0:
                textColor = Color.green;
                txt = "Easy";
                break;
            case 1:
                textColor = Color.yellow;
                txt = "Normal";
                break;
            case 2:
                textColor = Color.red;
                txt = "Hard";
                break;
            default:
                textColor = Color.red;
                txt = "Extreme";
                break;
        }   

        DifficultyText.text = txt;
        DifficultyText.color = textColor;

        LowerDiff.enabled = gameDiff > 1 || (gameDiff > 0 && playerProgress >= 7);
        AddDiff.enabled = gameDiff < 3;
    }
}
