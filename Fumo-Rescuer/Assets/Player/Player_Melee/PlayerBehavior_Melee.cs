using UnityEngine;

public class PlayerBehavior_Melee : PlayerBehaviorScript
{
    public float skillDuration = 7.0f;
    public float skillDurationCountdown = 0f;

    public int healingPerSkillCast = 50;
    public float SkillCooldown = 18.0f;
    public float timeSinceSkillUse = 5.0f;

    public bool isUsingSkill = false;

    public PlayerBehavior_Melee()
    {
        movementLockout = 0.45f;
    }

    public override E_DamageType GetCharacterDamageType()
    {
        return E_DamageType.PHYSIC;
    }

    public override void Start()
    {
        base.Start();
        SkillEffect.SetActive(false);
    }

    public override void Update()
    {
        if (currentHealth <= 0) return;

        if (isUsingSkill)
        {
            if (skillDurationCountdown > 0)
            {
                LoopSkill();
            }
            else
            {
                EndSkill();
            }
        }
        base.Update();
    }

    public override void GetCharacterActions()
    {
        base.GetCharacterActions();

        if (Input.GetKeyDown(KeyCode.X) && timeSinceSkillUse >= SkillCooldown)
        {
            StartSkill();
        }
    }

    public override void UpdateCharacterCooldowns()
    {
        base.UpdateCharacterCooldowns();
        timeSinceSkillUse += Time.deltaTime;
        skillReady.enabled = (timeSinceSkillUse >= SkillCooldown);
    }

    public void StartSkill()
    {
        isUsingSkill = true;
        currentHealth = Mathf.Min(currentHealth + healingPerSkillCast, maxHealth);
        healthBar.setHealth(currentHealth);
        skillDurationCountdown = skillDuration;
        timeSinceSkillUse = 0.0f;

        attackDamage = (int)(baseAtk * 1.4f);
        def = baseDef * 3;
        res = baseRes + 200;
        speed = baseSpeed * 1.35f;
        
        SkillEffect.SetActive(true);
    }

    public void LoopSkill()
    {
        skillDurationCountdown -= Time.deltaTime;
    }

    public void EndSkill()
    {
        isUsingSkill = false;
        attackDamage = baseAtk;
        def = baseDef;
        res = baseRes;
        speed = baseSpeed;
        SkillEffect.SetActive(false);
    }
}
