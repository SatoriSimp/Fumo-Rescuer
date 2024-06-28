using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehavior_Sentinel : EnemyBehaviorScript
{
    [SerializeField] private List<Transform> CheckPoints;
    private AudioSource AlarmSFX;
    private Transform RangeIndicator;

    private int CheckpointsCount;
    private int CurrentCheckpoint;
    [SerializeField] private float stayTime;

    public override void Start()
    {
        base.Start();
        damageType = E_DamageType.PHYSIC;
        attackPattern = E_AttackPattern.SENTINEL;

        AlarmSFX = GetComponent<AudioSource>();
        RangeIndicator = attackPoint.GetComponentInChildren<Transform>();

        CheckpointsCount = CheckPoints.Count;
        timeSinceLastMovement = 9999;
    }

    public override void Update()
    {
        if (currentHealth <= 0) return;
        UpdateCharacterCooldowns();
        CheckAndFlipSpriteIfNecessary();
        Roam();
        DetectTargetsInRangeAndAttack();
    }

    void Roam()
    {
        if (CheckPoints.Count <= 0) return;

        if (movementDisabledCountdown > 0 || timeSinceLastMovement < moveTime + stayTime) return;

        Vector3 MovePoint = CheckPoints.ElementAt(CurrentCheckpoint).position;
        moveTime = Vector3.Distance(transform.position, MovePoint) / moveDistance;

        StartCoroutine(MoveToTarget(transform.position, MovePoint, moveTime));
    }

    public override void DetectTargetsInRangeAndAttack()
    {
        targetsInRange = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerMask);
        validTargets = targetsInRange.Where(t => t.gameObject != this.gameObject).ToArray();

        if (validTargets.Length > 0 && !playerDetected)
        {
            playerDetected = validTargets[0].transform;
            animator.SetTrigger("skill");
            if (!AlarmSFX.isPlaying) AlarmSFX.Play();
            RangeIndicator.localScale *= 100;

            movementDisabledCountdown = 1f;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
            foreach (GameObject enemy in enemies)
            {
                EnemyBehaviorScript e = enemy.GetComponent<EnemyBehaviorScript>();
                e.playerDetected = validTargets[0].transform;

                if (e.gameObject.GetComponent<EnemyBehavior_Sentinel>())
                {
                    e.GetComponent<Animator>().SetTrigger("skill");
                    e.movementDisabledCountdown = 1f;
                }
            }
        }
    }

    public override IEnumerator MoveToTarget(Vector2 startPosition, Vector2 endPosition, float time)
    {
        CurrentCheckpoint++;
        if (CurrentCheckpoint >= CheckpointsCount) CurrentCheckpoint = 0;

        timeSinceLastMovement = 0f;
        flipSprite = startPosition.x < endPosition.x;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            if (movementDisabledCountdown > 0 || currentHealth <= 0)
            {
                if (currentHealth > 0) animator.SetFloat("move", 0);
                CurrentCheckpoint--;
                if (CurrentCheckpoint < 0) CurrentCheckpoint = CheckpointsCount - 1;
                break;
            }

            animator.SetFloat("move", 2);
            transform.position = UnityEngine.Vector2.Lerp(startPosition, endPosition, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        animator.SetFloat("move", 0);
    }

}
