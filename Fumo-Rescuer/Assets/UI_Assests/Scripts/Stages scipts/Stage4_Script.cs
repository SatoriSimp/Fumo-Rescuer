using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossStageScript : MonoBehaviour
{
    public GameObject BossGameObject;
    public GameObject CageGameObject;

    public Transform BossSpawnPosition;

    private EnemyBehavior_Assassin Boss;
    private PlayerBehaviorScript Player;

    [SerializeField] private SpriteRenderer BackgroundRenderer;
    private bool FadedBG = false;
    private Color InitBgColor = Color.white;
    private Color DarkenBgColor = new(0.385f, 0.385f, 0.385f);


    private void Start()
    {
        Player = GameObject.FindFirstObjectByType<PlayerBehaviorScript>();
        StartCoroutine(SpawnBoss());
    }

    private IEnumerator SpawnBoss()
    {
        float spawnDuration = 3;
        float spawnDurationCount = 0;

        while (spawnDurationCount < spawnDuration)
        {
            spawnDurationCount += Time.deltaTime;
            yield return null;
        }

        GameObject bossObj = Instantiate(BossGameObject, BossSpawnPosition.position, Quaternion.identity);
        Boss = bossObj.GetComponent<EnemyBehavior_Assassin>();
        Boss.playerDetected = Player.transform;
        StartCoroutine(Boss.FadeIn(4));
    }

    private void Update()
    {
        if (!Boss || !CageGameObject) return; 
    
        if (Boss.Phase == 2 && !FadedBG)
        {
            FadedBG = true;
            StartCoroutine(ChangeBackgroundColor(InitBgColor, DarkenBgColor, 10));
        }

        if (!Boss.IsAlive())
        {
            Destroy(CageGameObject);
            StartCoroutine(ChangeBackgroundColor(DarkenBgColor, InitBgColor, 3));
        }
    }

    IEnumerator ChangeBackgroundColor(Color startColor, Color endColor, float duration)
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            Color current = Color.Lerp(startColor, endColor, elapsed / duration);
            BackgroundRenderer.color = current;

            yield return null;
            elapsed += Time.deltaTime;
        }

        BackgroundRenderer.color = endColor;
    }
}
