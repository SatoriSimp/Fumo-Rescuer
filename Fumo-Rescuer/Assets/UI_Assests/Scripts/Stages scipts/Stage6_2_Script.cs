using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage6_2_Script : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject BlackScreenOverlay;
    public GameObject BaitFumo;

    public GameObject EoE_Prefab;
    public GameObject RealFumo;
    public Transform spawnpoint;

    private GameObject EoE;
    private bool EoESpawned = false;
    private bool triggeredEoESpawn = false;

    AudioSource BGM;
    bool fumSpawned = false;

    void Start()
    {
        BGM = GetComponent<AudioSource>();
        StartCoroutine(FadeOut(BlackScreenOverlay, 3));
    }

    // Update is called once per frame
    void Update()
    {
        if (!EoESpawned) return;

        if (!EoE && !fumSpawned)
        {
            fumSpawned = true;
            Instantiate(RealFumo, spawnpoint.position, Quaternion.identity);
        }
    }

    void SpawnEoE()
    {
        if (triggeredEoESpawn) return;

        if (BGM && !BGM.isPlaying) BGM.Play();

        triggeredEoESpawn = true;

        PlayerBehaviorScript player = FindFirstObjectByType<PlayerBehaviorScript>();
        player.movementLockoutCountup = -5;
        player.animator.SetFloat("run", 0);

        StartCoroutine(DestroyFumoAndSpawnEoE());
    }

    public IEnumerator FadeOut(GameObject obj, float fadeDuration)
    {
        float counter = 0;
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        Color spriteColor = renderer.color;

        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / fadeDuration);

            renderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(fadeDuration);
    }

    public IEnumerator FadeIn(GameObject obj, float fadeDuration)
    {
        float counter = 0;
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
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

    IEnumerator DestroyFumoAndSpawnEoE()
    {
        yield return FadeOut(BaitFumo, 2);
        Destroy(BaitFumo);
        yield return new WaitForSeconds(1);
        EoE = Instantiate(EoE_Prefab, spawnpoint.position, Quaternion.identity);
        EoESpawned = true;
        yield return FadeIn(EoE, 5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggeredEoESpawn) return;

        if (collision && collision.CompareTag("Player"))
        {
            SpawnEoE();
        }
    }
}
