using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GuideSection : MonoBehaviour
{
    public GameObject[] Sections;
    
    public Button NextButton, BackButton;

    private short CurrentActiveSection = 0;

    private void Start()
    {
        UpdateActiveSection();
    }

    public void ChangeSection(bool IsChangingToNextSection)
    {
        CurrentActiveSection = (short) (IsChangingToNextSection ? CurrentActiveSection + 1 : CurrentActiveSection - 1);
        UpdateActiveSection();
    }

    private void UpdateActiveSection()
    {
        for (int i = 0; i < Sections.Length; i++)
        {
            if (i != CurrentActiveSection) Sections[i].SetActive(false);
            else Sections[i].SetActive(true);
        }

        BackButton.enabled = CurrentActiveSection != 0;
        NextButton.enabled = CurrentActiveSection != (Sections.Length - 1);
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
