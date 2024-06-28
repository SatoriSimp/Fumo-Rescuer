using System.Collections;
using UnityEngine;

public class EnemyBehavior_Artillery : EnemyBehaviorScript
{
    public GameObject bullet;

    public bool isSkillStarting = false;
    public float SkillCooldown = 10;
    public float timeSinceLastSkillUse = 0;

    public float skillTime = 8.0f;
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.PHYSIC;
        attackPattern = E_AttackPattern.RANGED;
        if (isMenuShowcaseObject) initialSpriteFlipped = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        if (currentHealth <= 0) return;

        if (!isSkillStarting) base.Update();

        if (playerDetected)
        {
            moveDistance = 40f;
            moveCooldown = 5f;
        }

        timeSinceLastSkillUse += Time.deltaTime;
        if (timeSinceLastSkillUse >= SkillCooldown && validTargets.Length > 0 && !startAttacking) 
        {
            UseSkill();
        }
    }

    public void UseSkill() 
    {
        Debug.Log("Skill start!");
        isSkillStarting = true;
        Collider2D skillTarget = validTargets[0];
        if (!skillTarget) return;
        
        StartCoroutine(CastSkill(skillTarget.GetComponent<PlayerBehaviorScript>()));
        timeSinceLastSkillUse = 0;
    }

    IEnumerator CastSkill(PlayerBehaviorScript target)
    {
        animator.SetTrigger("skill");

        float skillTimeCountup = 0f;
        float bombardStartTimemark = 2.5f;
        float bombardEndTimemark = 6.5f;
        float bombardInterval = 0.25f; // Time between bombardments
        float bombardCount = 0f;

        while (skillTimeCountup < skillTime)
        {
            skillTimeCountup += Time.deltaTime;

            if (target && skillTimeCountup >= bombardStartTimemark && skillTimeCountup <= bombardEndTimemark)
            {
                bombardCount += Time.deltaTime;

                if (bombardCount >= bombardInterval)
                {
                    DealDamage((int) (attackDamage * 0.35f), target);
                    bombardCount = 0; // Update next bombardment time
                }
            }

            yield return new WaitForSeconds(0);
        }

        isSkillStarting = false;
    }

    public override IEnumerator MoveToTarget(UnityEngine.Vector2 startPosition, UnityEngine.Vector2 endPosition, float time)
    {
        timeSinceLastMovement = 0f;
        flipSprite = startPosition.x < endPosition.x;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            if (movementDisabledCountdown > 0 || startAttacking || isSkillStarting || currentHealth <= 0) break;

            animator.SetFloat("move", 2);
            transform.position = UnityEngine.Vector2.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        animator.SetFloat("move", 0);
    }

    public override void DealDamage(int damage, PlayerBehaviorScript player)
    {
        GameObject proj = Instantiate(bullet, player.transform.position, Quaternion.identity);
        if (isSkillStarting) proj.GetComponent<SpriteRenderer>().color = Color.magenta;
        Destroy(proj, 0.5f);

        base.DealDamage(damage, player);
    }
}
