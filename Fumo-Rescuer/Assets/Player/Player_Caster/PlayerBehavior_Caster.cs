using TMPro;
using UnityEngine;

public class PlayerBehavior_Caster : PlayerBehaviorScript
{
    public GameObject ShootProjectile;
    public GameObject onskill_ShootProjectile;
    [SerializeField] protected GameObject AttackRangeIndicator;

    public TMP_Text NoTargetInRangeWarning;

    public short targetsLimitPerAttack = 1;
    public float skillDuration = 5f;
    private float skillDurationCountdown = 0f;
    public float skillCooldown = 30f;
    public float timeSinceLastSkillUsage = 20f;

    public float onSkill_attackSpeed = 0.2f;
    public float onSkill_timeSinceLastAttack = 0;
    public float onSkill_attackDamage;
    public bool skill_starting = false;
    AudioSource skillSFX;

    public override void Start()
    {
        base.Start();
        skillSFX = GetComponent<AudioSource>();
        SkillEffect.SetActive(false);
        NoTargetInRangeWarning.enabled = false;
        AttackRangeIndicator.SetActive(false);

        skillSFX.volume = PlayerPrefs.GetFloat("SFX_Volume", 1.0f);
    }

    public override E_DamageType GetCharacterDamageType()
    {
        return E_DamageType.MAGIC;
    }

    public override void UpdateCharacterCooldowns()
    {
        base.UpdateCharacterCooldowns();

        skillDurationCountdown -= Time.deltaTime;
        timeSinceLastSkillUsage += Time.deltaTime;
        onSkill_timeSinceLastAttack += Time.deltaTime;

        skillReady.enabled = timeSinceLastSkillUsage >= skillCooldown;

        if (!isAttacking) NoTargetInRangeWarning.enabled = false;
    }

    public override void Update()
    {
        if (currentHealth <= 0) return;
        UpdateCharacterCooldowns();

        if (skillDurationCountdown > 0) {
            LoopSkill();
        }
        else
        {
            if (skill_starting) EndSkill();
            GetCharacterMovement();
            GetCharacterActions();
        }
    }

    public override void GetCharacterActions()
    {
        base.GetCharacterActions();

        if (Input.GetKeyDown(KeyCode.X) && timeSinceLastSkillUsage >= skillCooldown)
        {
            StartSkill();
        }
    }

    public void StartSkill()
    {
        if (skillSFX)
        {
            skillSFX.Stop();
            skillSFX.Play();
        }
        animator.SetTrigger("skill");
        skillDurationCountdown = 5f;
        timeSinceLastSkillUsage = 0f;
        attackDamage = attackDamage * 6 / 10;
        skill_starting = true;
        SkillEffect.SetActive(true);
        AttackRangeIndicator.SetActive(true);
    }

    public void LoopSkill()
    {
        if (onSkill_timeSinceLastAttack < onSkill_attackSpeed) return;
        animator.SetTrigger("skill");

        hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        short targetDamaged = 0;
        foreach (Collider2D enemyHit in hitEnemies)
        {
            if (targetDamaged > 4) break;

            if (enemyHit)
            {
                GameObject MagicProjectile = Instantiate(onskill_ShootProjectile, attackPoint.position, attackPoint.rotation);
                MagicProjectile.GetComponent<CasterProjectileScript>().PlayerCaster = this;
                MagicProjectile.GetComponent<CasterProjectileScript>().ShootTowards(enemyHit);
                targetDamaged++;
            }
        }
        onSkill_timeSinceLastAttack = 0f;
    }

    public void EndSkill() {
        animator.SetFloat("run", 0);
        attackDamage = attackDamage * 10 / 6;
        skill_starting = false;
        SkillEffect.SetActive(false);
        AttackRangeIndicator.SetActive(false);
    }

    public override void Attack()
    {
        AttackRangeIndicator.SetActive(true);
        hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        
        NoTargetInRangeWarning.enabled = hitEnemies.Length <= 0;

        animator.SetTrigger("attack");
        timeSinceLastAttack = 0.0f;
        isAttacking = true;
    }

    public override void DamageFindTargets()
    {
        short targetDamaged = 0;
        foreach (Collider2D enemyHit in hitEnemies)
        {
            if (targetDamaged >= targetsLimitPerAttack) break;

            if (enemyHit)
            {
                GameObject MagicProjectile = Instantiate(ShootProjectile, attackPoint.position, Quaternion.identity);
                MagicProjectile.GetComponent<CasterProjectileScript>().PlayerCaster = this;
                MagicProjectile.GetComponent<CasterProjectileScript>().ShootTowards(enemyHit);
                targetDamaged++;
            }
        }
        isAttacking = false;
        AttackRangeIndicator.SetActive(false);
    }
}
