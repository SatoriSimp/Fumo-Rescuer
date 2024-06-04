using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehavior_Flayer : EnemyBehaviorScript
{
    public float MeleeAttackRange = 125;
    public GameObject HeirsPrefab;

    private Collider2D[] validTargets_Melee = { };
    private bool isPerformingMeleeAttack = false;
    private float baseInterval;

    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.PHYSIC;
        attackPattern = E_AttackPattern.RANGED;
        baseInterval = 0.4f;
    }

    public override void Update()
    {
        if (currentHealth <= 0) return;

        if (playerDetected)
        {
            moveCooldown = 2;
            moveDistance = 80;
        }

        if (isPerformingMeleeAttack) attackInterval = baseInterval + 0.1f;
        else attackInterval = baseInterval;

        base.Update();
    }

    public override void DetectTargetsInRangeAndAttack()
    {
        if (!startAttacking)
        {
            targetsInRange = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);
            validTargets_Melee = Physics2D.OverlapCircleAll(attackPoint.position, MeleeAttackRange, playerMask);
            validTargets = targetsInRange.Where(t => t.gameObject != this.gameObject).ToArray();

            if (validTargets.Length > 0 && !playerDetected)
            {
                playerDetected = validTargets[0].transform;
            }
        }
        else
        {
            attackAnimationStart += Time.deltaTime;
            if (attackAnimationStart >= attackInterval)
            {
                DamageFindTargets();
                attackAnimationStart = 0;
            }
        }

        if (timeSinceLastAttack >= attackSpeed && (validTargets.Length > 0 || validTargets_Melee.Length > 0))
        {
            if (validTargets_Melee.Length > 0) 
            {
                animator.SetTrigger("attack_2");
                isPerformingMeleeAttack = true;

            }
            else if (validTargets.Length > 0)
            {
                animator.SetTrigger("attack");
            }

            startAttacking = true;
            timeSinceLastAttack = 0;
            movementDisabledCountdown = attackInterval * 2f;
        }
    }

    public override void DamageFindTargets()
    {
        if (!isPerformingMeleeAttack) base.DamageFindTargets();
        else
        {
            targetsInRange = Physics2D.OverlapCircleAll(attackPoint.position, MeleeAttackRange, playerMask);
            validTargets = targetsInRange.Where(t => t.gameObject != this.gameObject).ToArray();
            this.defPen += 50;
            foreach (Collider2D target in validTargets)
            {
                if (!target || target.gameObject == this.gameObject) continue;

                DealDamage((int) (attackDamage * 2.5f), target.GetComponent<PlayerBehaviorScript>());
            }
            this.defPen -= 50;
            startAttacking = false;
        }
        isPerformingMeleeAttack = false;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        if (HeirsPrefab)
        {
            for (int i = 1; i <= 2; ++i)
            {
                GameObject HeirSpawned = Instantiate(HeirsPrefab, transform.position, Quaternion.identity);
                if (playerDetected)
                {
                    HeirSpawned.GetComponent<EnemyBehaviorScript>().playerDetected = playerDetected;
                }
            }
            HeirsPrefab = null;
        }
    }
}
