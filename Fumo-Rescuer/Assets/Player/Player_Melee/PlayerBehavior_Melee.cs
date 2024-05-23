using UnityEngine;

public class PlayerBehavior_Melee : PlayerBehaviorScript
{
    public int healingPerSkillCast = 30;
    public float healingCooldown = 12.5f;
    public float timeSinceLastHealing = 5.0f;

    public PlayerBehavior_Melee()
    {
        movementLockout = 0.45f;
    }

    public override E_DamageType GetCharacterDamageType()
    {
        return E_DamageType.PHYSIC;
    }

    public override void GetCharacterActions()
    {
        base.GetCharacterActions();

        if (Input.GetKeyDown(KeyCode.X) && timeSinceLastHealing >= healingCooldown)
        {
            Heal();
        }
    }

    public override void UpdateCharacterCooldowns()
    {
        base.UpdateCharacterCooldowns();
        timeSinceLastHealing += Time.deltaTime;
        skillReady.enabled = (timeSinceLastHealing >= healingCooldown);
    }

    public void Heal()
    {
        currentHealth = Mathf.Min(currentHealth + healingPerSkillCast, maxHealth);
        healthBar.setHealth(currentHealth);
        timeSinceLastHealing = 0.0f;
    }
}
