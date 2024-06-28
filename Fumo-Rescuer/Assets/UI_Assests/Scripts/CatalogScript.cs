using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CatalogScript : MonoBehaviour
{
    private AudioSource SFX;

    public GameObject[] EnemyButtons;
    private List<Transform> EnemyButtonBorders = new();
    public GameObject[] EnemyInfoBoxes;

    private int HighestCurrentLevel;
    private int[] LevelRequireToUnlock = {
        1, 1, 3, 3, 3, 3, 3, 5, 5, 6, 6, 4, 7
    };

    [SerializeField] private Image NoDataInfoBox;
    [SerializeField] private Image NoDataIcon;

    // Start is called before the first frame update
    void Start()
    {
        HighestCurrentLevel = PlayerPrefs.GetInt("HighestLevelCompleted");
        SFX = GetComponent<AudioSource>();

        for (int i = 0; i < EnemyButtons.Length; ++i)
        {
            Transform imgComp = EnemyButtons[i].transform.Find("Border");

            EnemyButtonBorders.Add(imgComp);
        }

        foreach (Transform border in EnemyButtonBorders)
        {
            border.gameObject.SetActive(false);
        }

        foreach (var infobox in EnemyInfoBoxes)
        {
            infobox.SetActive(false);
        }

        for (int i = 0; i < EnemyButtons.Length; ++i)
        {
            if (LevelRequireToUnlock[i] > HighestCurrentLevel + 1)
            {
                EnemyButtonBorders[i].GetComponent<Image>().color = Color.black;
                EnemyButtonBorders[i].localScale = Vector3.one;
                DisplayNoData(EnemyButtons[i], EnemyInfoBoxes[i], i >= 11);
            }
        }
    }

    public void ShowEnemyInfoAtIndex(int index)
    {
        if (SFX)
        {
            SFX.Stop();
            SFX.Play();
        }

        for (int i = 0; i < EnemyButtons.Length; i++)
        {
            EnemyButtonBorders[i].gameObject.SetActive(i == index);
            EnemyInfoBoxes[i].SetActive(i == index);
        }
    }

    void DisplayNoData(GameObject EntityButton, GameObject Entity, bool isElite)
    {
        EntityButton.GetComponent<Image>().color = Color.black;
        Transform buttonIcon = EntityButton.transform.Find("Icon");
        buttonIcon.GetComponent<Image>().sprite = NoDataIcon.sprite;
        buttonIcon.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
        buttonIcon.localScale = new(1.05f, 1.05f);
        
        EntityButton.GetComponent<Image>().color = Color.black;


        Transform infobox = Entity.transform.Find("InfoBox");

        Transform Title = infobox.Find("Title");
        Transform name = Title.Find("Name");
        name.GetComponent<TMP_Text>().text = "No Data";
        name.GetComponent<TMP_Text>().fontSize = 72;

        Transform icon = Title.Find("Icon");
        icon.GetComponent<Image>().sprite = NoDataInfoBox.sprite;
        icon.localScale = new(1f, 1f);

        if (isElite)
        {
            Transform elite = Title.Find("EliteSymbol");
            elite.gameObject.SetActive(false);

            Transform SkillSet = infobox.Find("Skill");
            Transform Content = SkillSet.Find("Scroll View").Find("Viewport").Find("Content");

            TMP_Text[] Skills = Content.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text skill in Skills) skill.text = "";

            Transform Des = Content.Find("Des");
            Des.GetComponent<TMP_Text>().text =
                "The journey is still far from over." +
                "\nKeep exploring to unlock the information for this enemy.";
            Des.GetComponent<TMP_Text>().fontSize = 42;
        }
        else
        {
            Transform Des = infobox.Find("Des");
            Des.GetComponent<TMP_Text>().text =
                "The journey is still far from over." +
                "\nKeep exploring to unlock the information for this enemy.";
            Des.GetComponent<TMP_Text>().fontSize = 42;

            Transform Skill = infobox.Find("Skill");
            Skill.GetComponent<TMP_Text>().text = "";
        }

        Transform Stats = infobox.transform.Find("Stats");
        Transform[] AllStats =
        {
            Stats.Find("Statbox_HP"),
            Stats.Find("Statbox_ATK"),
            Stats.Find("Statbox_DEF"),
            Stats.Find("Statbox_RES"),
            Stats.Find("Statbox_ASPD"),
            Stats.Find("Statbox_AINT"),
            Stats.Find("Statbox_MSPD"),
            Stats.Find("Statbox_DmgType"),
            Stats.Find("Statbox_Pattern")
        };

        foreach (Transform stat in AllStats)
        {
            stat.GetComponentInChildren<TMP_Text>().text = "???";
            stat.GetComponentInChildren<TMP_Text>().color = Color.white;
            stat.GetComponentInChildren<TMP_Text>().fontSize = 60;
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
