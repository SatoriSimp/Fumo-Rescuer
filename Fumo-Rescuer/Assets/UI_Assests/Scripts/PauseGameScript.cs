using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGameScript : MonoBehaviour
{
    private Canvas PauseUI;
    private bool GameIsPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        PauseUI = GetComponentInChildren<Canvas>();
        PauseUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) Pause();
            else Resume();

            GameIsPaused = !GameIsPaused;
        }
    }

    void Pause() 
    { 
        Time.timeScale = 0f;
        PauseUI.gameObject.SetActive(true);
    }

    void Resume()
    {
        Time.timeScale = 1f;
        PauseUI.gameObject.SetActive(false);
    }

    public void Retry()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Resume();
        SceneManager.LoadSceneAsync(0);
    }
}
