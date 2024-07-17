using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenSizeSetting : MonoBehaviour
{
    /*
    TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.value = PlayerPrefs.GetInt("WindowSize", 2);
        SetWindowSize();
    }

    public void SetWindowSize()
    {
        int index = dropdown.value;

        switch (index)
        {
            case 0:
                Screen.SetResolution(800, 600, false);
                break;
            case 1:
                Screen.SetResolution(1024, 768, false);
                break;
            case 2:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                break;
        }
        PlayerPrefs.SetInt("WindowSize", index);
    }
    */

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
}
