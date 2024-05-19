using System.Security.Cryptography;
using TMPro;
using UnityEngine;


public class PlayerBehaviorScript : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public float speed = 100f;
    private float currentSpeed;

    public int attackDamage = 15;
    public float attackRange = 40f;
    public float attackSpeed = 1f;
    public float attackInterval = 0.45f;
    public float timeSinceLastAttack = 1.0f;

    public float movementLockout;
    public float movementLockoutCountdown = 0;

    public float defPen, defIgn, resPen, resIgn; 
    public float def = 100, res = 0;

    public enum E_DamageType
    {
        TRUE = 0,
        PHYSIC = 1,
        MAGIC = 2,
    }

    public E_DamageType damageType = E_DamageType.PHYSIC;

    public int healingPerSkillCast = 30;
    public float healingCooldown = 12.5f;
    public float timeSinceLastHealing = 5.0f;

    private Collider2D hitbox;
    private bool isFacingRight;
   
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public HealthBarScript healthBar;
    public Rigidbody2D rb;
    public TMP_Text skillReady;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        attackPoint = gameObject.GetComponent<Transform>();
        hitbox = gameObject.GetComponent<Collider2D>();
        isFacingRight = true;
        currentHealth = maxHealth;
        currentSpeed = speed;
        healthBar.setMaxHealth(maxHealth);
        healthBar.setHealth(currentHealth);
    }

    void Update()
    {
        if (currentHealth <= 0) return;
        movementLockoutCountdown += Time.deltaTime;
        if (movementLockoutCountdown < movementLockout) return;

        timeSinceLastAttack += Time.deltaTime;
        timeSinceLastHealing += Time.deltaTime;
        skillReady.enabled = (timeSinceLastHealing >= healingCooldown);

        GetCharacterMovement();
    }

    // Movement and handles player inputs
    void GetCharacterMovement() 
    {
        if (timeSinceLastAttack >= attackInterval)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            rb.velocity = new Vector2(moveHorizontal, moveVertical);
            transform.Translate(currentSpeed * Time.deltaTime * rb.velocity);

            isFacingRight = moveHorizontal >= 0;
            if (moveHorizontal != 0) FlipPlayerSprite();

            animator.SetFloat("run", Mathf.Abs(moveHorizontal) + Mathf.Abs(moveVertical));

            if (Input.GetKeyDown(KeyCode.Z) && timeSinceLastAttack >= attackSpeed)
            {
                Attack();
            }
            else if (Input.GetKeyDown(KeyCode.X) && timeSinceLastHealing >= healingCooldown)
            {
                Heal();
            }
        }
    }

    void FlipPlayerSprite()
    {
        spriteRenderer.flipX = !isFacingRight;
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

    void Attack()
    {
        animator.SetTrigger("attack");
        Collider2D[] hitEnemies =
            Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemyHit in hitEnemies)
        {
            if (enemyHit != hitbox)
            {
                Debug.Log(enemyHit.name + " hit!");
                DealDamage(attackDamage, enemyHit.GetComponent<EnemyBehaviorScript>());
            }
        }
        timeSinceLastAttack = 0.0f;
    }

    public void Heal()
    {
        currentHealth = Mathf.Min(currentHealth + healingPerSkillCast, maxHealth);
        healthBar.setHealth(currentHealth);
        timeSinceLastHealing = 0.0f;
    }

    // Collisions handlers
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the character has collided with a terrain
        if (collision.gameObject.CompareTag("GameBorder"))
        {
            currentSpeed = 0.5f;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // When the character stops colliding, set the isColliding flag back to false
        currentSpeed = speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("FumoTrophy"))
        {
            Destroy(collision.gameObject);
            GameObject.FindGameObjectWithTag("Win").GetComponent<WinningScript>().PlayerVictoryConfirmed();
        }
    }
}
