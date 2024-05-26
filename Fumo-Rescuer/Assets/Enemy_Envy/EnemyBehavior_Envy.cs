using System.Collections;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class EnemyBehavior_Envy : EnemyBehaviorScript
{
    public GameObject EnvyProjectile;

    public float SkillCooldown = 20f;
    public float TimeSinceLastSkillUse = 0;
    public bool isMoving = false;

    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.MAGIC;
        attackPattern = E_AttackPattern.RANGED;
        if (isMenuShowcaseObject) initialSpriteFlipped = true;
    }

    public override void Update()
    {
        if (currentHealth <= 0) return;
        if (playerDetected)
        {
            moveCooldown = 2f;
        }
        base.Update();
    }

    public override void DamageFindTargets()
    {
        if (attackPattern == E_AttackPattern.MELEE)
        {
            targetsInRange = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);
            validTargets = targetsInRange.Where(t => t.gameObject != this.gameObject).ToArray();
        }

        foreach (Collider2D target in validTargets)
        {
            if (!target || target.gameObject == this.gameObject) continue;

            GameObject ShootProjectile = Instantiate(EnvyProjectile, attackPoint.position, attackPoint.rotation);
            ShootProjectile.GetComponent<EnvyProjectileScript>().Envy = this;
            ShootProjectile.GetComponent<EnvyProjectileScript>().ShootTowards(target);
        }

        startAttacking = false;
    }

    public override void AttemptToMove()
    {
        if (timeSinceLastMovement >= moveCooldown && movementDisabledCountdown <= 0 && !startAttacking)
        {
            isMoving = true;
            animator.SetFloat("move", 200);
            
            if (!playerDetected)
            {
                StartCoroutine(MoveToTarget(transform.position, new UnityEngine.Vector2(transform.position.x - moveDistance, transform.position.y), moveTime));
            }
            else
            {
                float distanceToPlayer = UnityEngine.Vector2.Distance(transform.position, playerDetected.position);

                // Calculate the direction to move away from the player
                UnityEngine.Vector2 direction = (transform.position - playerDetected.position).normalized;

                // Calculate the start and end positions
                UnityEngine.Vector2 startPosition = transform.position;

                UnityEngine.Vector2 endPosition = 
                    (distanceToPlayer <= attackRange)
                    ?
                        (transform.position + (UnityEngine.Vector3) direction * (attackRange * 1.1f))
                    :
                        (transform.position - (UnityEngine.Vector3) direction * (attackRange * 1.1f));

                StartCoroutine(MoveToTarget(startPosition, endPosition, Mathf.Max(UnityEngine.Vector2.Distance(startPosition, endPosition) / Mathf.Abs(moveDistance), 2)));
            }
        }
    }

    public override IEnumerator MoveToTarget(UnityEngine.Vector2 startPosition, UnityEngine.Vector2 endPosition, float time)
    {
        StartCoroutine(base.MoveToTarget(startPosition, endPosition, time));
        isMoving = false;
        yield return null;
    }

    public override void UpdateCharacterCooldowns()
    {
        base.UpdateCharacterCooldowns();
        TimeSinceLastSkillUse += Time.deltaTime;        
    }
}