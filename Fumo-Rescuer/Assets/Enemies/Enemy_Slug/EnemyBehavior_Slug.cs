using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EnemyBehavior_Slug : EnemyBehaviorScript
{
    AudioSource AttackSFX;
    public float lifeStealRatio = 0.8f;
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.MAGIC;
        attackPattern = E_AttackPattern.MELEE;
        AttackSFX = GetComponent<AudioSource>();

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
        if (AttackSFX) AttackSFX.Play();
        base.DealDamage(damage, player);
        Heal((int) (attackDamage * lifeStealRatio));
    }
}
