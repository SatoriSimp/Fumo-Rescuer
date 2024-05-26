using System.Collections;
using UnityEngine;

public class PlayerCaster_Menu : PlayerBehavior_Caster
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        isAttacking = false;
        skill_starting = false;
        StartCoroutine(MoveToTarget(transform.position, new Vector2(transform.position.x + 4000, transform.position.y), 85));
    }

    public override void Update()
    {

    }

    public IEnumerator MoveToTarget(UnityEngine.Vector2 startPosition, UnityEngine.Vector2 endPosition, float time)
    {

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            animator.SetFloat("run", 2);
            transform.position = UnityEngine.Vector2.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        animator.SetFloat("run", 0);
    }
}
