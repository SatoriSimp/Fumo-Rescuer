using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehavior_Assassin : EnemyBehaviorScript
{
    public GameObject Tooltips2;

    public short Phase = 1;
    public short SpecialAttackCost = 3;
    private short SpecialAttackCount = 1;

    AudioSource attackSFX, specialAttackSFX, invisibleSFX;

    public GameObject specialAttackRangeIndicator;
    public short specialAttackRange = 250;

    private bool usingSpecialAttack = false, usingSkill = false;

    private float missingHealthPercentage;

    private PlayerBehaviorScript player;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.PHYSIC;
        attackPattern = E_AttackPattern.MELEE;
        specialAttackRangeIndicator.SetActive(false);

        AudioSource[] sfxs = GetComponents<AudioSource>();
        attackSFX = sfxs[0];
        specialAttackSFX = sfxs[1];
        invisibleSFX = sfxs[2];
    }

    // Update is called once per frame
    public override void Update()
    {
        if (currentHealth <= 0 && Phase == 2) return;

        if (usingSpecialAttack || usingSkill) return;

        if (playerDetected)
        {
            if (!player) player = playerDetected.GetComponent<PlayerBehaviorScript>();
            moveDistance = 90f;
            moveCooldown = 5;
        }

        missingHealthPercentage = (maxHealth - currentHealth) * 1.0f / maxHealth;

        switch (Phase)
        {
            case 1:
                base.Update();
                break;
            case 3:
                Phase_2_Action();
                break;
        }
    }

    public override void DetectTargetsInRangeAndAttack()
    {
        if (!startAttacking)
        {
            targetsInRange = (
                SpecialAttackCount >= SpecialAttackCost 
                ?
                Physics2D.OverlapCircleAll(attackPoint.position, specialAttackRange, playerMask)
                :
                Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask)
            );

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

        if (timeSinceLastAttack >= attackSpeed)
        {
            if (validTargets.Length > 0)
            {
                if (SpecialAttackCount >= SpecialAttackCost) 
                {
                    StartCoroutine(UseSpecialAttack(0));
                }
                else
                {
                    if (attackSFX)
                    {
                        attackSFX.Play();
                    }
                    animator.SetTrigger("attack");
                    movementDisabledCountdown = attackInterval * 2f;
                    SpecialAttackCount++;
                }
                startAttacking = true;
                timeSinceLastAttack = 0;
            }
        }
    }

    public void Phase_2_Action()
    {
        moveDistance = 40;
        attackSpeed = 9.0f - (4.0f * missingHealthPercentage);

        AttemptToMove();
        CheckAndFlipSpriteIfNecessary();

        timeSinceLastAttack += Time.deltaTime;
        if (timeSinceLastAttack >= attackSpeed && playerDetected && playerDetected.GetComponent<PlayerBehaviorScript>().currentHealth > 0) 
        {
            StartCoroutine(DashAndAttack());
            timeSinceLastAttack = 0;
        }
    }

    public override void TakeDamage(int P_Damage, int M_Damage, int T_Damage, PlayerBehaviorScript player)
    {
        if (Phase == 2 || invulnerable) return;

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
            if (Phase == 3)
            {
                animator.SetTrigger("die");
                this.enabled = false;
                hitbox.enabled = false;
                playerInRange.enabled = false;
                healthBar.Hide();
                Destroy(gameObject, 5);
            }
            else
            {
                Phase = 2;
                StartCoroutine(Revive());
            }
        }
    }

    IEnumerator UseSpecialAttack(float duration)
    {
        if (specialAttackSFX)
        {
            specialAttackSFX.Stop();
            specialAttackSFX.Play();
        }
        specialAttackRangeIndicator.SetActive(true);
        animator.SetTrigger("attack_2");
        usingSpecialAttack = true;

        float attackDuration = 2.2f;
        float damageTimeMark = 1.1f;
        float durationCount = 0;
        bool damaged = false;

        while (durationCount < attackDuration)
        {
            timeSinceLastAttack += Time.deltaTime;
            durationCount += Time.deltaTime;

            if (currentHealth <= 0) break;

            if (!damaged && durationCount >= damageTimeMark)
            {
                validTargets = Physics2D.OverlapCircleAll(attackPoint.position, specialAttackRange, playerMask);
                foreach (Collider2D target in validTargets)
                {
                    if (!target) continue;
                    DealDamage((int) (attackDamage * 1.5f), 
                        target.GetComponent<PlayerBehaviorScript>()
                    );
                }
                damaged = true;
            }

            yield return null;
        }

        specialAttackRangeIndicator.SetActive(false);
        SpecialAttackCount -= SpecialAttackCost;
        startAttacking = false;
        usingSpecialAttack = false;
        yield return new WaitForSeconds(duration);

    }

    IEnumerator Revive()
    {
        float reviveDuration = 10;
        float reviveCount = 0;

        animator.SetTrigger("die");
        StartCoroutine(FadeOut(reviveDuration));
        Instantiate(Tooltips2);

        while (reviveCount < reviveDuration)
        {
            reviveCount += Time.deltaTime;
            currentHealth = maxHealth * Mathf.Min((int) (reviveCount * 10), 100) / 100;
            healthBar.setHealth(currentHealth);
            yield return null;
        }

        currentHealth = maxHealth;
        healthBar.setHealth(currentHealth);
        Phase = 3;

        animator.ResetTrigger("die");
        animator.SetTrigger("revive");

        def *= 0.3f;
        res -= 200;
        defPen += 25;

        movementDisabledCountdown = 0;
        startAttacking = false;
        usingSpecialAttack = false;
        usingSkill = false;
        timeSinceLastAttack = 4;
        attackSpeed = 9;
        invulnerable = true;
    }

    public IEnumerator FadeOut(float duration)
    {
        float counter = 0;
        Color spriteColor = renderer.color;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0.1f, counter / duration);

            renderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(duration);
    }

    public IEnumerator FadeIn(float fadeDuration)
    {
        float counter = 0;
        Color spriteColor = renderer.color;

        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(0.1f, 1, counter / fadeDuration);

            renderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(fadeDuration);
    }

    public IEnumerator Dash()
    {
        animator.SetTrigger("skill");
        Vector2 DashPostion = (
            playerDetected.transform.position.x < transform.position.x 
            ?
            new Vector2(playerDetected.transform.position.x - moveDistance + MoveOffset.x, playerDetected.transform.position.y + MoveOffset.y)
            :
            new Vector2(playerDetected.transform.position.x + moveDistance + MoveOffset.x, playerDetected.transform.position.y + MoveOffset.y)
        );
        yield return new WaitForSeconds(0.35f);

        transform.position = DashPostion;
        validTargets = Physics2D.OverlapCircleAll(attackPoint.position, specialAttackRange, playerMask);
        foreach (Collider2D target in validTargets)
        {
            if (!target) continue;

            int dashDamage = (int) 
            (
                player 
                ? 
                (attackDamage * 0.33f) 
                : 
                (attackDamage * (0.33f + ((player.maxHealth - player.currentHealth) * 100 / player.maxHealth * 0.0075f)))
            );
            
            DealDamage(dashDamage,
                target.GetComponent<PlayerBehaviorScript>()
            );

            Debug.Log("Dash attack dealt " + dashDamage + " damage!");
        }
        CheckAndFlipSpriteIfNecessary();
    }

    public IEnumerator DashAndAttack()
    {
        if (invisibleSFX)
        {
            invisibleSFX.Stop();
            invisibleSFX.Play();
        }
        yield return new WaitForSeconds(1f);

        yield return FadeIn(0.5f);
        usingSkill = true;
        invulnerable = false;

        yield return Dash();
        yield return new WaitForSeconds(0.25f);
        yield return UseSpecialAttack(0);

        usingSkill = false;
        invulnerable = true;

        yield return FadeOut(0.5f);
        yield return new WaitForSeconds(1f);
    }

    public bool IsAlive()
    {
        return currentHealth > 0 || Phase != 3;
    }
}
