using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EnemyBehavior_Slug : EnemyBehaviorScript
{
    public float lifeStealRatio = 0.15f;
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.MAGIC;
        attackPattern = E_AttackPattern.MELEE;

        if (isMenuShowcaseObject) initialSpriteFlipped = true;
    }

    public override void Update()
    {
        if (currentHealth <= 0) return;
        if (playerDetected)
        {
            moveCooldown = 0f;
            moveDistance = 300f;
        }
        base.Update();
    }

    public override void DealDamage(int damage, PlayerBehaviorScript player)
    {
        base.DealDamage(damage, player);
        Heal((int) (attackDamage * lifeStealRatio));
    }
}
