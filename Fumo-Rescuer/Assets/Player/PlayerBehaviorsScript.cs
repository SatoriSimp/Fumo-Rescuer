using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;


public class PlayerBehaviorScript : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public float speed = 100f;
    private float currentSpeed;

    public int attackDamage = 15;
    public float attackRange = 40f;
    public float attackSpeed = 1f;
    public float attackInterval = 0.45f;
    public float timeSinceLastAttack = 1.0f;

    public float movementLockout = 0.45f;
    public float movementLockoutCountup = 0;

    public float defPen, defIgn, resPen, resIgn;
    public float def = 100, res = 0;

    public float invulnerable = 0;

    private bool StartFunctionExecuted = false;

    public enum E_DamageType
    {
        TRUE = 0,
        PHYSIC = 1,
        MAGIC = 2,
    }

    public E_DamageType damageType;

    public Collider2D hitbox;
    public Collider2D[] hitEnemies = { };

    public bool isAttacking = false;
    public bool isFacingRight;

    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public HealthBarScript healthBar;
    public Rigidbody2D rb;
    public TMP_Text skillReady;

    public virtual void Start()
    {
        if (StartFunctionExecuted) return;

        StartFunctionExecuted = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        attackPoint = gameObject.GetComponent<Transform>();
        hitbox = gameObject.GetComponent<Collider2D>();
        damageType = GetCharacterDamageType();
        isFacingRight = true;
        currentHealth = maxHealth;
        currentSpeed = speed;
        healthBar.setMaxHealth(maxHealth);
        healthBar.setHealth(currentHealth);
    }

    public virtual E_DamageType GetCharacterDamageType()
    {
        return E_DamageType.TRUE;
    }

    public virtual void Update()
    {
        if (currentHealth <= 0) return;
        UpdateCharacterCooldowns();

        GetCharacterMovement();
        GetCharacterActions();
    }

    public virtual void UpdateCharacterCooldowns()
    {
        timeSinceLastAttack += Time.deltaTime;
        movementLockoutCountup += Time.deltaTime;
        invulnerable -= Time.deltaTime;
    }

    // Movement and handles player inputs
    public virtual void GetCharacterMovement()
    {
        if (movementLockoutCountup < movementLockout) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(moveHorizontal, moveVertical);
        transform.Translate(currentSpeed * Time.deltaTime * rb.velocity);

        isFacingRight = moveHorizontal >= 0;
        if (moveHorizontal != 0) FlipPlayerSprite();

        animator.SetFloat("run", Mathf.Abs(moveHorizontal) + Mathf.Abs(moveVertical));
    }

    public virtual void FlipPlayerSprite()
    {
        spriteRenderer.flipX = !isFacingRight;
    }

    public virtual void GetCharacterActions()
    {
        if (Input.GetKeyDown(KeyCode.Z) && timeSinceLastAttack >= attackSpeed)
        {
            Attack();
            movementLockoutCountup = 0f;
        }

        if (timeSinceLastAttack >= attackInterval && isAttacking)
        {
            DamageFindTargets();
        }
    }

    // Combat functions
    public void DealDamage(int Pdamage, int Mdamage, int Tdamage, EnemyBehaviorScript target)
    {
        target.TakeDamage(Pdamage, Mdamage, Tdamage, this);
    }

    public void DealDamage(int damage, EnemyBehaviorScript target)
    {
        if (!target) return;
        switch (damageType)
        {
            case E_DamageType.PHYSIC:
                DealDamage(damage, 0, 0, target);
                break;
            case E_DamageType.MAGIC:
                DealDamage(0, damage, 0, target);
                break;
            case E_DamageType.TRUE:
                DealDamage(0, 0, damage, target);
                break;
        }
    }

    public virtual void TakeDamage(int P_Damage, int M_Damage, int T_damage, EnemyBehaviorScript player)
    {
        if (invulnerable > 0) return;

        int damage = (int)(
            P_Damage * (1 - Mathf.Max(Mathf.Min(def * (1 - player.defPen * 0.01f) - player.defIgn, 950), 0) * 0.001f)
            + M_Damage * (1 - Mathf.Max(Mathf.Min(res * (1 - player.resPen * 0.01f) - player.resIgn, 900), 0) * 0.001f)
            + T_damage
        );

        currentHealth -= damage;
        healthBar.setHealth(currentHealth);

        if (currentHealth <= 0)
        {
            animator.SetTrigger("die");
            hitbox.enabled = false;
            healthBar.enabled = false;
            Destroy(gameObject, 5);
        }
    }

    public virtual void Attack()
    {
        animator.SetTrigger("attack");
        timeSinceLastAttack = 0.0f;
        isAttacking = true;
    }

    public virtual void DamageFindTargets()
    {
        hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D enemyHit in hitEnemies)
        {
            if (!enemyHit || enemyHit != hitbox)
            {
                Debug.Log(enemyHit.name + " hit!");
                DealDamage(attackDamage, enemyHit.GetComponent<EnemyBehaviorScript>());
            }
        }
        isAttacking = false;
    }


    // Collisions handlers
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the character has collided with a terrain
        if (collision.gameObject.CompareTag("GameBorder"))
        {
            currentSpeed = 0.5f;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        currentSpeed = speed;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("FumoTrophy"))
        {
            Destroy(collision.gameObject);
            GameObject.FindGameObjectWithTag("Win").GetComponent<WinningScript>().PlayerVictoryConfirmed();
        }
    }
}
