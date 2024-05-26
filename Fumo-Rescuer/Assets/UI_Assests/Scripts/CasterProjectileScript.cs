using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterProjectileScript : MonoBehaviour
{
    public PlayerBehavior_Caster PlayerCaster;
    public float travelSpeed = 25f;

    private Collider2D Target;

    public void ShootTowards(Collider2D enemyCollider)
    {
        Target = enemyCollider;
        Destroy(gameObject, 5); // force destroy to prevent projectile from stucking in the air
    }

    private void Update()
    {
        if (!Target) Destroy(gameObject);
        // Move the bullet towards the target
        transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, travelSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemies"))
        {
            if (collision) PlayerCaster.DealDamage(PlayerCaster.attackDamage, collision.gameObject.GetComponent<EnemyBehaviorScript>());
            Destroy(gameObject);
        }
    }
}
