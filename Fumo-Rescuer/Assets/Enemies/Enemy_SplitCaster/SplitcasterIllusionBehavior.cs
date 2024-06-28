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

    public override void OnDeath()
    {
        Destroy(gameObject);
    }
}
