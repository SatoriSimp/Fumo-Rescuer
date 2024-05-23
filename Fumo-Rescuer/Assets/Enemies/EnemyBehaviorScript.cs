using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

abstract public class EnemyBehaviorScript : MonoBehaviour
{
    public int maxHealth, currentHealth;

    public int attackDamage;
    public enum E_DamageType {
        PHYSIC = 1,
        MAGIC = 2,
        TRUE = 0,
    };

    public E_DamageType damageType;

    public float attackSpeed;
    public float timeSinceLastAttack = 0.0f;

    public float attackInterval;
    public float attackAnimationStart = 0.0f;
    public float attackRange;

    public float defPen, defIgn, resPen, resIgn;
    public float def, res;

    public short numberOfMoves;
    public float moveCooldown;
    public float moveTime;
    public float moveDistance;
    public float timeSinceLastMovement;
    public bool startAttacking = false;
    public float movementDisabledCountdown = 0f;

    public Animator animator;
    public Collider2D hitbox;
    public Transform attackPoint;
    public LayerMask playerMask;
    public HealthBarScript healthBar;
    public Rigidbody2D rb;
    public TMP_Text playerInRange;
    public Transform playerDetected = null;

    public virtual void TakeDamage(int P_Damage, int M_Damage, int T_Damage, PlayerBehaviorScript player)
    {
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
            healthBar.enabled = false;
            Destroy(gameObject, 5);
        }
    }

    public virtual void TakeDamage(int P_Damage, int M_Damage, int T_Damage, EnemyBehaviorScript player)
    {
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
            healthBar.enabled = false;
            Destroy(gameObject, 5);
        }
    }

    public virtual void TakeDamage(int P_Damage, int M_Damage, int T_Damage)
    {
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
            playerInRange.enabled = false;
            healthBar.enabled = false;
            Destroy(gameObject, 5);
        }
    }

    public void DealDamage(int Pdamage, int Mdamage, int Tdamage,  PlayerBehaviorScript player) 
    {
        if (!player) return;
        player.TakeDamage(Pdamage, Mdamage, Tdamage, this);   
    }

    public void DealDamage(int damage, PlayerBehaviorScript player)
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

    public void DealDamage(int Pdamage, int Mdamage, int Tdamage, EnemyBehaviorScript player)
    {
        if (!player) return;
        player.TakeDamage(Pdamage, Mdamage, Tdamage, this);
    }

    public void DealDamage(int damage, EnemyBehaviorScript player)
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
}
