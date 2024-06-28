using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior_EoE : EnemyBehaviorScript
{
    private short Phase = 0;

    // From left to right:
    // Whip attack -> Summon wisps -> Knockback -> Explosion
    public GameObject[] ExtraTooltips;

    public WispSpawnpoint[] wispSpawnpoints_Straight, wispSpawnpoints_Diag;

    public readonly float[][] AbilityCooldowns =
    {
        new float[] { 12.0f, 20.0f },
        new float[] { 9.0f, 12.0f },
        new float[] { 6.0f, 5.0f, 5.0f },
    };

    private readonly float[][] AbilityCooldownsCountup =
    {
        new float[]{ 10, 12 },
        new float[]{ 6, 6 },
        new float[]{ 0, 0, 0 },
    };

    // All phases assests
    [SerializeField] private GameObject SpikeAttack;
    [SerializeField] private GameObject SlugPrefab;
    [SerializeField] private GameObject ShatteredGroundPrefab;

    // P2 assets
    [SerializeField] private float SweepAttackDistance;

    // P3 assets
    [SerializeField] private float ExplosionRadius;
    [SerializeField] private int ExplosionDamage;
    private TyrantShatteredGround ShatteredGround;

    private bool isCastingSkill = false;
    private bool isReviving = false;
    private bool deathTriggered = false;
    private float HPLossTimerCountup = 0;

    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.PHYSIC;
        attackPattern = E_AttackPattern.RANGED;
        displayTooltipsOnSpawn = false;
        Phase = 0;
        playerDetected = FindFirstObjectByType<PlayerBehaviorScript>().transform;
        currentHealth = 1;
        healthBar.setHealth(1);

        StartCoroutine(Revive(10));

        foreach (WispSpawnpoint s in wispSpawnpoints_Straight) s.gameObject.SetActive(false);
        foreach (WispSpawnpoint s in wispSpawnpoints_Diag) s.gameObject.SetActive(false);
    }

    public override void Update()
    {
        if (currentHealth <= 0 || isReviving) return;

        UpdateCooldowns();

        if (isCastingSkill) return;

        switch (Phase)
        {
            case 1:
                UpdatePhase_1();
                break;
            case 2:
                UpdatePhase_2();
                break;
            case 3:
                UpdatePhase_3();
                break;
        }
    }

    void UpdatePhase_1()
    {
        if (AbilityCooldownsCountup[0][1] >= AbilityCooldowns[0][1])
        {
            SummonSlug(1);
            AbilityCooldownsCountup[0][1] = 0;
        }
        else if (AbilityCooldownsCountup[0][0] >= AbilityCooldowns[0][0])
        {
            if (playerDetected)
            {
                DoWhipAttack();
            }
            AbilityCooldownsCountup[0][0] = 0;
        }
    }

    void UpdatePhase_2()
    {
        if (AbilityCooldownsCountup[1][1] >= AbilityCooldowns[1][1])
        {
            SummonSlug(1);
            AbilityCooldownsCountup[1][1] = 0;
        }
        else if (AbilityCooldownsCountup[1][0] >= AbilityCooldowns[1][0])
        {
            if (playerDetected)
            {
                DoWhipAttack();
            }
            AbilityCooldownsCountup[1][0] = 0;
        }
    }

    void UpdatePhase_3()
    {
        HPLossTimerCountup += Time.deltaTime;

        if (HPLossTimerCountup >= 1 && playerDetected)
        {
            TakeDamage(0, 0, 6, this);
            HPLossTimerCountup = 0;
            if (ShatteredGround)
            {
                ShatteredGround.transform.localScale += Vector3.one * 1.5f;
                ShatteredGround.radius += 12;
            }
        }

        if (AbilityCooldownsCountup[2][2] >= AbilityCooldowns[2][2])
        {
            StartCoroutine(PullPlayerTowardsSelf(5));
            AbilityCooldownsCountup[2][2] = 0;
        }
        if (AbilityCooldownsCountup[2][1] >= AbilityCooldowns[2][1])
        {
            SummonSlug(1);
            AbilityCooldownsCountup[2][1] = 0;
        }
        else if (AbilityCooldownsCountup[2][0] >= AbilityCooldowns[2][0])
        {
            if (playerDetected)
            {
                DoWhipAttack();
            }
            AbilityCooldownsCountup[2][0] = 0;
        }
    }

    void UpdateCooldowns()
    {
        for (short i = 0; i < AbilityCooldowns[Phase - 1].Length; i++)
        {
            AbilityCooldownsCountup[Phase - 1][i] += Time.deltaTime;
        }
    }

    public override void OnDeath()
    {
        if (isReviving) return;

        foreach (WispSpawnpoint w in wispSpawnpoints_Straight) w.gameObject.SetActive(false);
        foreach (WispSpawnpoint w in wispSpawnpoints_Diag) w.gameObject.SetActive(false);

        if (Phase < 3 && currentHealth <= 0)
        {
            StartCoroutine(Revive(5));
        }
        else if (!deathTriggered)
        {
            deathTriggered = true;

            EnemyBehaviorScript[] Enemies = FindObjectsOfType<EnemyBehaviorScript>();
            foreach (EnemyBehaviorScript enemy in Enemies)
            {
                if (!enemy) continue;
                enemy.TakeDamage(0, 0, 9999, this);
            }

            if (ShatteredGround) Destroy(ShatteredGround);

            base.OnDeath();
        }
    }

    IEnumerator Revive(float duration)
    {
        Instantiate(ExtraTooltips[Phase]);
        
        if (Phase != 0) animator.SetTrigger("die");
        
        isReviving = true;
        invulnerable = true;

        float count_up = 0;
        while (count_up < duration)
        {
            short cHealth = (short)(maxHealth * count_up / duration);
            currentHealth = cHealth;
            healthBar.setHealth(cHealth);

            yield return null;
            count_up += Time.deltaTime;
        }

        Phase++;
        animator.SetInteger("phase", Phase);
        currentHealth = maxHealth;
        healthBar.setHealth(maxHealth);
        isReviving = false;
        invulnerable = false;

        if (Phase >= 2)
        {
            def = 350;
            foreach (WispSpawnpoint w in wispSpawnpoints_Straight) w.gameObject.SetActive(true);
            if (Phase >= 3)
            {
                foreach (WispSpawnpoint w in wispSpawnpoints_Straight)
                {
                    w.SpawnInterval = 3;
                    w.WispMoveTime = 5;
                }
                foreach (WispSpawnpoint w in wispSpawnpoints_Diag)
                {
                    w.SpawnInterval = 3;
                    w.WispMoveTime = 4;
                    w.gameObject.SetActive(true);
                }
                def = 1000;
                res = 1000;

                GameObject shtgrnd = Instantiate(ShatteredGroundPrefab, transform.position, Quaternion.identity);
                shtgrnd.transform.localScale = Vector3.one * 3;
                ShatteredGround = shtgrnd.GetComponent<TyrantShatteredGround>();
                ShatteredGround.radius = 30;
                ShatteredGround.timeLast = 9999;
            }
        }
    }

    void DoWhipAttack()
    {
        if (!playerDetected) return;

        animator.SetTrigger("skill");

        Vector3 enemyPos = attackPoint.position, playerPos = playerDetected.position;
        Vector3 direction = playerPos - enemyPos;
        float distance = direction.magnitude;
        direction.Normalize();

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Create the whip attack area
        GameObject whipAttack = Instantiate(SpikeAttack, enemyPos, Quaternion.identity);
        whipAttack.transform.localScale *= 1 + (0.2f * Phase);
        whipAttack.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Adjust the position to align with the direction
        whipAttack.transform.position = enemyPos + direction * (distance / 2);
    }

    void SummonSlug(int number)
    {
        for (int i = 1; i <= number; i++)
        {
            GameObject slugGmObj = Instantiate(SlugPrefab, transform.position, Quaternion.identity);
            slugGmObj.GetComponent<EnemyBehaviorScript>().playerDetected = this.playerDetected;
        }
    }

    IEnumerator PullPlayerTowardsSelf(float duration)
    {
        if (!playerDetected) yield return null;

        float count = 0;
        float pullSpeed = 75;

        while (count < duration)
        {
            if (!playerDetected || Vector2.Distance(playerDetected.position, transform.position) <= 0.1f) break;
            
            playerDetected.position = Vector2.MoveTowards(playerDetected.position, transform.position, pullSpeed * Time.deltaTime);
            yield return null;

            count += Time.deltaTime;
        }
    }
}
