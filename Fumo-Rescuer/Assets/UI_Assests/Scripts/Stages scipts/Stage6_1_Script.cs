using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage6_1_Script : MonoBehaviour
{
    public List<GameObject> Guards;
    public List<GameObject> Gates;

    private bool AssassinTriggerActivated = false;

    [SerializeField]
    private GameObject FirstSectionGoal;

    [SerializeField]
    private GameObject BlackScreenOverlay;

    // Update is called once per frame
    void Update()
    {
        GameObject MarkedToRemove = null;

        foreach (GameObject g in Guards)
        {
            if (g.GetComponent<EnemyBehaviorScript>().currentHealth <= 0)
            {
                MarkedToRemove = g;
                GameObject gt = Gates.ElementAt(Guards.IndexOf(g));
                if (gt != null) 
                { 
                    Gates.Remove(gt);
                    Destroy(gt);
                }
            }
        }

        if (MarkedToRemove)
        {
            Guards.Remove(MarkedToRemove);
        }
    }

    public void TriggerWinAndMoveToNextSection()
    {
        PlayerBehaviorScript player = GameObject.FindFirstObjectByType<PlayerBehaviorScript>();
        player.invulnerable = 999;
        player.movementLockoutCountup = -999;
        StartCoroutine(FadeIn(2, BlackScreenOverlay));
        SceneManager.LoadSceneAsync("Level_6_2");

        SaveProgress();
    }

    void SaveProgress()
    {
        int CurrentLevel = SceneManager.GetActiveScene().buildIndex - 3;

        if (PlayerPrefs.GetInt("HighestLevelCompleted") < CurrentLevel)
        {
            PlayerPrefs.SetInt("HighestLevelCompleted", CurrentLevel);
            PlayerPrefs.Save();
        }
    }

    public IEnumerator FadeIn(float fadeDuration, GameObject Object)
    {
        float counter = 0;
        SpriteRenderer renderer = Object.GetComponent<SpriteRenderer>();
        Color spriteColor = renderer.color;

        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, counter / fadeDuration);

            renderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(fadeDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (AssassinTriggerActivated) return; 

        if (collision && collision.gameObject.CompareTag("Player"))
        {
            AssassinTriggerActivated = true;
            EnemyBehavior_Assassin Assassin = FindFirstObjectByType<EnemyBehavior_Assassin>();
            if (Assassin != null) 
            {
                Assassin.playerDetected = collision.gameObject.transform;
            }
        }
    }
}
