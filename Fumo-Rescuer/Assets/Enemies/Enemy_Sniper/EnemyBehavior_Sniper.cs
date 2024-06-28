using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehavior_Sniper : EnemyBehaviorScript
{
    [SerializeField] private GameObject ArrowProjectile;

    public override void Start()
    {
        base.Start();

        damageType = E_DamageType.PHYSIC;
        attackPattern = E_AttackPattern.RANGED;
    }

    // Update is called once per frame
    public override void Update()
    {
        if (currentHealth <= 0) return;
        if (playerDetected) attackRange = 10_000;
        base.Update();
    }

    public override void AttemptToMove()
    {

    }

    public override void DamageFindTargets()
    {

        foreach (Collider2D target in validTargets)
        {
            if (!target || target.gameObject == this.gameObject) continue;

            GameObject Projectile = Instantiate(ArrowProjectile, attackPoint.position, Quaternion.identity);
            SniperProjectileScript Arrow = Projectile.GetComponent<SniperProjectileScript>();

            Arrow.EnemySniper = this;
            Arrow.ShootTowards(target);
        }
        startAttacking = false;
    }
}
