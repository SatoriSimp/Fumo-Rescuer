using System.Collections;
using System.Linq;
using UnityEngine;

public class EnemyBehaviorScript_Melee : EnemyBehaviorScript
{
    public bool isSentinel = false;
    private bool isFacingRight; // default value for sprite is false

    private Collider2D[] targetsInRange = { }, validTargets = { };

    public bool isMenuShowcasseObject = false;

    public new SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        damageType = E_DamageType.PHYSIC;

        renderer = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<Collider2D>();
        currentHealth = maxHealth;
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
            (validTargets != null && validTargets.Length > 0) || playerDetected;

        FlipSprite();

        if (!startAttacking)
        {
            targetsInRange = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);
            validTargets = targetsInRange.Where(t => t.gameObject != this.gameObject).ToArray();

            if (validTargets.Length > 0 && !playerDetected)
            {
                playerDetected = validTargets[0].transform;
                moveCooldown = 3.0f;

                if (isSentinel)
                {
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
                    foreach (GameObject enemy in enemies)
                    {
                        EnemyBehaviorScript e = enemy.GetComponent<EnemyBehaviorScript>();
                        e.playerDetected = validTargets[0].transform;
                        e.moveCooldown = 2.0f;
                        e.moveTime = 2.0f;
                        e.moveDistance += 150f;
                    }
                }
            }
        }
        else
        {
            attackAnimationStart += Time.deltaTime;
            if (attackAnimationStart >= attackInterval)
            {
                damageFindTargets();
                attackAnimationStart = 0;
                movementDisabledCountdown = 0.5f;
            }
        }

        if (timeSinceLastAttack >= attackSpeed)
        {
            if (validTargets.Length > 0)
            {
                animator.SetTrigger("attack");
                startAttacking = true;
                movementDisabledCountdown = 0.5f;
                timeSinceLastAttack = 0;
            }
        }

        if ((timeSinceLastMovement >= moveCooldown) && validTargets.Length <= 0 && movementDisabledCountdown <= 0 && !startAttacking)
        {
            // Set the "move" parameter in the Animator
            animator.SetFloat("move", 2);
            if (!playerDetected)
                StartCoroutine(MoveToTarget(transform.position, new Vector2(transform.position.x - moveDistance, transform.position.y), moveTime));
            else
                StartCoroutine(MoveToTarget(transform.position, playerDetected.position, Mathf.Max(Vector2.Distance(transform.position, playerDetected.position) / Mathf.Abs(moveDistance), 1)));
        }
    }

    void FlipSprite()
    {
        if (movementDisabledCountdown <= 0 && playerDetected)
        {
            isFacingRight = (
                playerDetected
                && playerDetected.position.x > transform.position.x
            );
        }

        renderer.flipX = isFacingRight;
    }
    void damageFindTargets()
    {
        targetsInRange = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);
        validTargets = targetsInRange.Where(t => t.gameObject != this.gameObject).ToArray();
        
        foreach (Collider2D target in validTargets)
        {
            if (!target || target.gameObject == this.gameObject) continue;

            if (!isMenuShowcasseObject)
            {
                DealDamage(attackDamage, target.GetComponent<PlayerBehaviorScript>());
            }
            else DealDamage(attackDamage, target.GetComponent<EnemyBehaviorScript>());
        }
        startAttacking = false;
    }

    IEnumerator MoveToTarget(Vector2 startPosition, Vector2 endPosition, float time)
    {
        timeSinceLastMovement = 0f;
        isFacingRight = startPosition.x < endPosition.x;

        if (numberOfMoves > 0)
        {
            float elapsedTime = 0;

            while (elapsedTime < time)
            {
                if (elapsedTime > moveCooldown || movementDisabledCountdown > 0 || startAttacking || currentHealth <= 0) break;

                animator.SetFloat("move", 2);
                transform.position = Vector2.Lerp(startPosition, endPosition, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        numberOfMoves--;
        animator.SetFloat("move", 0);
    }
}
