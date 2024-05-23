using System.Collections;
using UnityEngine;

public class EnemyBehaviorScript_Sarkaz : EnemyBehaviorScript
{
    private bool isFacingRight; // default value for sprite is false

    private Collider2D[] targets;

    public new SpriteRenderer renderer;

    public EnemyBehaviorScript_Sarkaz()
    {
        damageType = E_DamageType.MAGIC;
    }

    // Start is called before the first frame update
    void Start()
    {
        damageType = E_DamageType.MAGIC;

        renderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        hitbox = GetComponent<Collider2D>();
        timeSinceLastAttack = attackSpeed;
        healthBar.setMaxHealth(maxHealth);
        playerInRange.enabled = false;
        isFacingRight = renderer.flipX;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0) return;

        timeSinceLastAttack += Time.deltaTime;
        timeSinceLastMovement += Time.deltaTime;
        movementDisabledCountdown -= Time.deltaTime;

        playerInRange.enabled = 
            (Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask).Length > 0) || startAttacking;

        // Flip the enemy's sprite corresponding to the player's position
        if (movementDisabledCountdown <= 0 && playerDetected)
        {
            isFacingRight = (
                playerDetected
                && playerDetected.position.x > transform.position.x
            );
        }

        renderer.flipX = isFacingRight;

        if (!startAttacking)
        {
            targets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);
            if (!playerDetected && targets.Length > 0)
            {
                playerDetected = targets[0].transform;
            }
        }
        else
        {
            attackAnimationStart += Time.deltaTime;
            if (attackAnimationStart >= attackInterval)
            {
                damageFindTargets();
                movementDisabledCountdown = attackInterval;
                startAttacking = false;
            }
        }

        if (timeSinceLastAttack >= attackSpeed) 
        {
            if (targets.Length > 0)
            {
                animator.SetTrigger("attack");
                startAttacking = true;
                timeSinceLastAttack = 0;
            }
        }



        if (timeSinceLastMovement >= moveCooldown && movementDisabledCountdown <= 0 && !startAttacking)
        {
            animator.SetFloat("move", 2);
            if (!playerDetected)
                StartCoroutine(MoveToTarget(transform.position, new Vector2(transform.position.x - moveDistance, transform.position.y), moveTime));
            else
                StartCoroutine(MoveToTarget(transform.position, playerDetected.position, Vector2.Distance(transform.position, playerDetected.position) / Mathf.Abs(moveDistance)));

        }
    }

    void damageFindTargets()
    {
        foreach (Collider2D target in targets)
        {
            if (!target) continue;
            DealDamage(attackDamage, target.GetComponent<PlayerBehaviorScript>());
        }
        attackAnimationStart = 0f;
    }

    IEnumerator MoveToTarget(Vector2 startPosition, Vector2 endPosition, float time)
    {
        timeSinceLastMovement = 0f;
        attackRange += 25f;

        isFacingRight = startPosition.x < endPosition.x;

        if (numberOfMoves > 0)
        {
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                if (movementDisabledCountdown > 0 || startAttacking || currentHealth <= 0) break;

                animator.SetFloat("move", 2);
                transform.position = Vector2.Lerp(startPosition, endPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        numberOfMoves--;
        animator.SetFloat("move", 0);
    }

    public override void TakeDamage(int P_Damage, int M_Damage, int T_Damage, PlayerBehaviorScript player)
    {
        if (!player) return;

        int damage = (int)(
            P_Damage * (1 - Mathf.Max(Mathf.Min(def * (1 - player.defPen * 0.01f) - player.defIgn, 950), 0) * 0.001f)
            + M_Damage * (1 - Mathf.Max(Mathf.Min(res * (1 - player.resPen * 0.01f) - player.resIgn, 900), 0) * 0.001f)
            + T_Damage
        );

        currentHealth -= damage;
        healthBar.setHealth(currentHealth);
        if (currentHealth > 0)
        {
            if (!startAttacking) animator.SetTrigger("hurt");
        }
        else
        {
            animator.SetTrigger("die");
            this.enabled = false;
            hitbox.enabled = false;
            playerInRange.enabled = false;
            healthBar.enabled = false;
            Destroy(gameObject, 6);
        }
    }
}
