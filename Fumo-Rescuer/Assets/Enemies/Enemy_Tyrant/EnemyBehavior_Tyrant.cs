using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_Tyrant : EnemyBehaviorScript
{
    public GameObject DoT_Zone;
    AudioSource AttackSFX;

    // Start is called before the first frame update
    public override void Start()
    {
        AttackSFX = GetComponent<AudioSource>();
        base.Start();
        damageType = E_DamageType.PHYSIC;
        attackPattern = E_AttackPattern.MELEE;
    }

    public override void Update()
    {
        if (currentHealth <= 0) return;

        if (playerDetected)
        {
            moveCooldown = 5f;
            moveDistance = 75;
        }

        base.Update();
    }

    public override void DamageFindTargets()
    {
        if (AttackSFX)
        {
            AttackSFX.Stop();
            AttackSFX.Play();
        }

        if (validTargets.Length > 0 && validTargets[0])
        {
            GameObject DoT = Instantiate(DoT_Zone, validTargets[0].transform.position, Quaternion.identity);
            DoT.GetComponent<TyrantShatteredGround>().Tyrant = this.gameObject;
        }
        base.DamageFindTargets();
    }
}
