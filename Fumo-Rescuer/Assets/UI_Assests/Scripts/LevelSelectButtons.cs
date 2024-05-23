using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButtons : MonoBehaviour
{
    public short LevelIndex;

    private void Start()
    {
        LevelIndex++;
        int currentNumberofLevels = SceneManager.sceneCountInBuildSettings - 1;
        if (LevelIndex > currentNumberofLevels)
        {
            this.GetComponent<Button>().enabled = false;
            this.GetComponent<Button>().interactable = false;
        }
    }

    public void OnClickLoadLevel()
    {
        SceneManager.LoadSceneAsync(LevelIndex);
        if (LevelIndex != 0) GameObject.FindFirstObjectByType<AudioScript>().StopMusic();
    }
}
