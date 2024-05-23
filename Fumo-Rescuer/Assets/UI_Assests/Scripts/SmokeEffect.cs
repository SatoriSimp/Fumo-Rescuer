using System.Collections;
using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    public float fadeTime = 2.0f;
    private new SpriteRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeAndDestroy(fadeTime));
    }

    private IEnumerator FadeAndDestroy(float fadeTime)
    {
        float fadeTimeCountup = 0f;
        Color spriteColor = renderer.color;

        while (fadeTimeCountup < fadeTime)
        {
            fadeTimeCountup += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, fadeTimeCountup / fadeTime);

            renderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}