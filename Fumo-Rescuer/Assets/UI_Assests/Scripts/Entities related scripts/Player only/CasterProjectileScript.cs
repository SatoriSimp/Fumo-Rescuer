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

        Vector3 targetDirection = Target.transform.position - transform.position;

        // Calculate the desired Z rotation angle (yaw)
        float desiredZRotation = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90;

        // Set the initial rotation with the desired Z angle
        transform.rotation = Quaternion.Euler(0f, 0f, desiredZRotation);

    }

    private void Update()
    {
        if (!Target) Destroy(gameObject);

        transform.position = (
            Vector3.MoveTowards(transform.position, Target.transform.position, travelSpeed)
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision && collision.gameObject.CompareTag("Enemies"))
        {
            if (collision) PlayerCaster.DealDamage(PlayerCaster.attackDamage, collision.gameObject.GetComponent<EnemyBehaviorScript>());
            Destroy(gameObject);
        }
    }
}
