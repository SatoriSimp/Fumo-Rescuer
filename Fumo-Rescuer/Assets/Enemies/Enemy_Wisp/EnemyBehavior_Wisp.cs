using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_Wisp : EnemyBehaviorScript
{
    public Vector3 endPosition;

    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.TRUE;
        attackPattern = E_AttackPattern.MELEE;
    }

    public override void Update()
    {
        if (transform.position == endPosition) Explode(null);
    }

    public void StartMoving()
    {
        StartCoroutine(MoveToTarget(transform.position, endPosition, moveTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Explode(collision.gameObject.GetComponent<PlayerBehaviorScript>());
        }
    }
    public override IEnumerator MoveToTarget(UnityEngine.Vector2 startPosition, UnityEngine.Vector2 endPosition, float time)
    {
        timeSinceLastMovement = 0f;
        flipSprite = startPosition.x < endPosition.x;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            animator.SetFloat("move", 2);
            transform.position = UnityEngine.Vector2.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Explode(null);
        animator.SetFloat("move", 0);
    }

    void Explode(PlayerBehaviorScript player)
    {
        if (player) DealDamage(attackDamage, player);
        OnDeath();
    }
}
