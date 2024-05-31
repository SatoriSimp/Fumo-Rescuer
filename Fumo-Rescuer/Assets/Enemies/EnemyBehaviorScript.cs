using System.Collections;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

abstract public class EnemyBehaviorScript : MonoBehaviour
{
    public Animator animator;
    public Collider2D hitbox;
    public Transform attackPoint;
    public LayerMask playerMask;
    public HealthBarScript healthBar;
    public Rigidbody2D rb;
    public TMP_Text playerInRange;
    public Transform playerDetected = null;
    public new SpriteRenderer renderer;

    public GameObject Tooltips;
    public bool displayTooltipsOnSpawn = false;

    public int maxHealth, currentHealth;

    public bool invulnerable = false;

    public int attackDamage;

    public enum E_DamageType {
        PHYSIC = 1,
        MAGIC = 2,
        TRUE = 0,
    };

    public enum E_AttackPattern
    {
        MELEE = 0,
        RANGED = 1,
        SENTINEL = 2,
    };

    public E_DamageType damageType;
    public E_AttackPattern attackPattern;

    public float attackSpeed;
    public float timeSinceLastAttack = 0.0f;

    public float attackInterval;
    public float attackAnimationStart = 0.0f;
    public float attackRange;

    public float defPen, defIgn, resPen, resIgn;
    public float def, res;

    public float moveCooldown;
    public float moveTime;
    public float moveDistance;
    public float timeSinceLastMovement;
    public bool startAttacking = false;
    public float movementDisabledCountdown = 0f;

    public UnityEngine.Vector3 MoveOffset = UnityEngine.Vector3.zero;

    private float baseMoveDistance;

    // Sentinel: alert all enemies upon spotting player unit
    public bool isSentinel = false;

    public bool initialSpriteFlipped; // default value is false, change depends on sprite
    public bool flipSprite = false;

    public Collider2D[] targetsInRange = { }, validTargets = { };

    public bool isMenuShowcaseObject = false;

    public virtual void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<Collider2D>();
        currentHealth = maxHealth;
        timeSinceLastAttack = attackSpeed;
        healthBar.setMaxHealth(maxHealth);
        playerInRange.enabled = false;
        initialSpriteFlipped = renderer.flipX && moveDistance >= 0;
        baseMoveDistance = moveDistance;
        if (displayTooltipsOnSpawn) DisplayTooltips();
    }

    public virtual void Update()
    {
        UpdateCharacterCooldowns();
        CheckAndFlipSpriteIfNecessary();
        DetectTargetsInRangeAndAttack();
        AttemptToMove();

        if (playerDetected)
        {
            moveCooldown = 
                attackPattern == E_AttackPattern.MELEE ? 0.5f : 2f;
            moveDistance = Mathf.Abs(moveDistance);
        }
    }

    public virtual void UpdateCharacterCooldowns()
    {
        timeSinceLastAttack += Time.deltaTime;
        timeSinceLastMovement += Time.deltaTime;
        movementDisabledCountdown -= Time.deltaTime;

        playerInRange.enabled =
            (Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask).Length > 0) || startAttacking;
    } 

    public virtual void CheckAndFlipSpriteIfNecessary()
    {
        if (movementDisabledCountdown <= 0 && playerDetected)
        {
            flipSprite = (
                playerDetected
                && playerDetected.position.x > transform.position.x
            );
        }

        renderer.flipX = (initialSpriteFlipped && !flipSprite) || (!initialSpriteFlipped && flipSprite);
    }

    public virtual void DetectTargetsInRangeAndAttack()
    {
        if (!startAttacking)
        {
            targetsInRange = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);
            validTargets = targetsInRange.Where(t => t.gameObject != this.gameObject).ToArray();

            if (validTargets.Length > 0 && !playerDetected)
            {
                playerDetected = validTargets[0].transform;

                if (isSentinel)
                {
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
                    foreach (GameObject enemy in enemies)
                    {
                        EnemyBehaviorScript e = enemy.GetComponent<EnemyBehaviorScript>();
                        e.playerDetected = validTargets[0].transform;
                        if (e.attackPattern == E_AttackPattern.MELEE)
                        {
                            e.moveDistance = e.baseMoveDistance + 100f;
                            e.moveCooldown = 1;
                            e.timeSinceLastMovement = 1;
                        }
                    }
                }
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

        if (timeSinceLastAttack >= attackSpeed)
        {
            if (validTargets.Length > 0)
            {
                animator.SetTrigger("attack");
                startAttacking = true;
                timeSinceLastAttack = 0;
                movementDisabledCountdown = attackInterval * 2f;
            }
        }
    }

    public virtual void AttemptToMove()
    {
        if (timeSinceLastMovement >= moveCooldown && movementDisabledCountdown <= 0 && !startAttacking)
        {
            animator.SetFloat("move", 2);
            if (!playerDetected)
                StartCoroutine(MoveToTarget(transform.position, new UnityEngine.Vector2(transform.position.x - moveDistance, transform.position.y), moveTime));
            else
                transform.position = UnityEngine.Vector2.MoveTowards(transform.position, playerDetected.position + MoveOffset, moveDistance * Time.deltaTime);
        }
        else animator.SetFloat("move", 0);
    }

    public virtual void DamageFindTargets()
    {
        if (attackPattern == E_AttackPattern.MELEE)
        {
            targetsInRange = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);
            validTargets = targetsInRange.Where(t => t.gameObject != this.gameObject).ToArray();
        }
        
        foreach (Collider2D target in validTargets)
        {
            if (!target || target.gameObject == this.gameObject) continue;

            DealDamage(attackDamage, target.GetComponent<PlayerBehaviorScript>());
        }
        startAttacking = false;
    }

    public virtual IEnumerator MoveToTarget(UnityEngine.Vector2 startPosition, UnityEngine.Vector2 endPosition, float time)
    {
        timeSinceLastMovement = 0f;
        flipSprite = startPosition.x < endPosition.x;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            if ((timeSinceLastMovement >= moveCooldown && movementDisabledCountdown <= 0) 
            || movementDisabledCountdown > 0 || startAttacking || currentHealth <= 0) 
                break;

            animator.SetFloat("move", 2);
            transform.position = UnityEngine.Vector2.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        animator.SetFloat("move", 0);
    }

    public virtual void TakeDamage(int P_Damage, int M_Damage, int T_Damage, PlayerBehaviorScript player)
    {
        if (invulnerable) return;

        int damage;
        
        if (player)
        {
            damage = (int)(
            P_Damage * (1 - Mathf.Max(Mathf.Min(def * (1 - player.defPen * 0.01f) - player.defIgn, 950), 0) * 0.001f)
            + M_Damage * (1 - Mathf.Max(Mathf.Min(res * (1 - player.resPen * 0.01f) - player.resIgn, 900), 0) * 0.001f)
            + T_Damage
            );
        }
        else
        {
            damage = (int)(
            P_Damage * (1 - Mathf.Max(Mathf.Min(def, 950), 0) * 0.001f)
            + M_Damage * (1 - Mathf.Max(Mathf.Min(res, 900), 0) * 0.001f)
            + T_Damage
            );
        }

        currentHealth -= damage;
        healthBar.setHealth(currentHealth);
        if (currentHealth <= 0)
        {
            animator.SetTrigger("die");
            this.enabled = false;
            hitbox.enabled = false;
            playerInRange.enabled = false;
            healthBar.Hide();
            Destroy(gameObject, 5);
        }
    }

    public virtual void TakeDamage(int P_Damage, int M_Damage, int T_Damage, EnemyBehaviorScript source)
    {
        if (invulnerable) return;

        int damage;

        if (source)
        {
            damage = (int)(
            P_Damage * (1 - Mathf.Max(Mathf.Min(def * (1 - source.defPen * 0.01f) - source.defIgn, 950), 0) * 0.001f)
            + M_Damage * (1 - Mathf.Max(Mathf.Min(res * (1 - source.resPen * 0.01f) - source.resIgn, 900), 0) * 0.001f)
            + T_Damage
            );
        }
        else
        {
            damage = (int)(
            P_Damage * (1 - Mathf.Max(Mathf.Min(def, 950), 0) * 0.001f)
            + M_Damage * (1 - Mathf.Max(Mathf.Min(res, 900), 0) * 0.001f)
            + T_Damage
            );
        }

        currentHealth -= damage;
        healthBar.setHealth(currentHealth);
        if (currentHealth <= 0)
        {
            animator.SetTrigger("die");
            this.enabled = false;
            hitbox.enabled = false;
            playerInRange.enabled = false;
            healthBar.Hide();
            Destroy(gameObject, 5);
        }
    }

    public virtual void TakeDamage(int P_Damage, int M_Damage, int T_Damage)
    {
        if (invulnerable) return;

        int damage = (int)(
            P_Damage * (1 - Mathf.Max(Mathf.Min(def, 950), 0) * 0.001f)
            + M_Damage * (1 - Mathf.Max(Mathf.Min(res, 900), 0) * 0.001f)
            + T_Damage
        );

        currentHealth -= damage;
        healthBar.setHealth(currentHealth);
        if (currentHealth <= 0)
        {
            animator.SetTrigger("die");
            this.enabled = false;
            hitbox.enabled = false;
            healthBar.Hide();
            playerInRange.enabled = false;
            Destroy(gameObject, 5);
        }
    }

    public virtual void DealDamage(int Pdamage, int Mdamage, int Tdamage,  PlayerBehaviorScript player) 
    {
        if (!player) return;

        player.TakeDamage(Pdamage, Mdamage, Tdamage, this);   
    }

    public virtual void DealDamage(int damage, PlayerBehaviorScript player)
    {
        if (!player) return;

        switch (damageType)
        {
            case E_DamageType.PHYSIC:
                DealDamage(damage, 0, 0, player);
                break;
            case E_DamageType.MAGIC:
                DealDamage(0, damage, 0, player);
                break;
            case E_DamageType.TRUE:
                DealDamage(0, 0, damage, player);   
                break;
        }
    }

    public virtual void DealDamage(int Pdamage, int Mdamage, int Tdamage, EnemyBehaviorScript player)
    {
        if (!player) return;
        player.TakeDamage(Pdamage, Mdamage, Tdamage, this);
    }

    public virtual void DealDamage(int damage, EnemyBehaviorScript player)
    {
        if (!player) return;
        switch (damageType)
        {
            case E_DamageType.PHYSIC:
                DealDamage(damage, 0, 0, player);
                break;
            case E_DamageType.MAGIC:
                DealDamage(0, damage, 0, player);
                break;
            case E_DamageType.TRUE:
                DealDamage(0, 0, damage, player);
                break;
        }
    }
    public virtual void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        healthBar.setHealth(currentHealth);
    }

    public void DisplayTooltips()
    {
        Instantiate(Tooltips);
    }
}
