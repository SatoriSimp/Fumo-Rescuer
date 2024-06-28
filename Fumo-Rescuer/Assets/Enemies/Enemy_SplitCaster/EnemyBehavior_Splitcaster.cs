using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_Splitcaster : EnemyBehaviorScript
{
    public GameObject IllusionPrefab;
    private GameObject Illusion;
    private bool CanSummonIllusion = true;

    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.MAGIC;
        attackPattern = E_AttackPattern.RANGED;
    }

    public override void Update()
    {
        if (currentHealth < 0) return;

        invulnerable = Illusion;

        if (currentHealth <= maxHealth / 2 && CanSummonIllusion) SummonIllusion();

        if (playerDetected)
        {
            moveDistance = 75;
            moveCooldown = 5;
        }

        base.Update();
    }

    void SummonIllusion()
    {
        CanSummonIllusion = false;
        float Distance = playerDetected ? Vector2.Distance(playerDetected.position, transform.position) : 0;

        Distance = Mathf.Min(Distance + 100, 300);

        if (playerDetected && playerDetected.position.x < transform.position.x) Distance *= -1;

        Vector2 IllusionSpawnPosition = new(transform.position.x - Distance, transform.position.y - Distance);

        Illusion = Instantiate(IllusionPrefab, IllusionSpawnPosition, Quaternion.identity);
    }
}
