using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrantShatteredGround : MonoBehaviour
{
    public GameObject AttackPoint;
    public LayerMask PlayerMask;

    public GameObject Tyrant;

    public int damagePerSec;
    public int radius;
    private int timeCount = 0;
    public int timeLast;

    private void Start()
    {
        Destroy(gameObject, timeLast);
    }

    private void FixedUpdate()
    {
        if (timeCount % 25 == 0)
        {
            DealDamage();
        }
        timeCount++;
    }

    void DealDamage()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(AttackPoint.transform.position, radius, PlayerMask);

        foreach (Collider2D target in targets)
        {
            if (!target) continue;
            Tyrant.GetComponent<EnemyBehaviorScript>().DealDamage(damagePerSec / 2, target.GetComponent<PlayerBehaviorScript>());
        }
    }
}
