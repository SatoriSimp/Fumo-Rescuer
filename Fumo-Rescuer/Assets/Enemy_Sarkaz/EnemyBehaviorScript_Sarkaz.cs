using System.Collections;
using UnityEngine;

public class EnemyBehaviorScript_Sarkaz : EnemyBehaviorScript
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.MAGIC;
        attackPattern = E_AttackPattern.RANGED;
    }

    public override IEnumerator MoveToTarget(Vector2 startPosition, Vector2 endPosition, float time)
    {
        attackRange += 25;
        return base.MoveToTarget(startPosition, endPosition, time);
    }

    public override void TakeDamage(int P_Damage, int M_Damage, int T_Damage, PlayerBehaviorScript player)
    {
        base.TakeDamage(P_Damage, M_Damage, T_Damage, player);

        if (currentHealth > 0)
        {
            if (!startAttacking) animator.SetTrigger("hurt");
        }
    }
}
