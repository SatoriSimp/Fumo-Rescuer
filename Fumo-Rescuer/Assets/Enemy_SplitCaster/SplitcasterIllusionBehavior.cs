using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitcasterIllusionBehavior : EnemyBehaviorScript
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        attackPattern = E_AttackPattern.SENTINEL;
    }

    // Update is called once per frame
    public override void Update()
    {
    }

    public override void TakeDamage(int P_Damage, int M_Damage, int T_Damage, PlayerBehaviorScript player)
    {
        if (invulnerable) return;

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
            Destroy(gameObject);
        }
    }
}
