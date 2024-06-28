using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonScript : MonoBehaviour
{
    public void SwitchToFirstLevel()
    {
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
    }
}
