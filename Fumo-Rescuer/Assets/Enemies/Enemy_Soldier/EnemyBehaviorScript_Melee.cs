using System.Linq;
using UnityEngine;

public class EnemyBehaviorScript_Melee : EnemyBehaviorScript
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.PHYSIC;
        attackPattern = E_AttackPattern.MELEE;
    }

    public override void DamageFindTargets()
    {
        targetsInRange = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);
        validTargets = targetsInRange.Where(t => t.gameObject != this.gameObject).ToArray();
        
        foreach (Collider2D target in validTargets)
        {
            if (!target || target.gameObject == this.gameObject) continue;

            DealDamage(attackDamage, target.GetComponent<PlayerBehaviorScript>());
        }
        startAttacking = false;
    }
}
