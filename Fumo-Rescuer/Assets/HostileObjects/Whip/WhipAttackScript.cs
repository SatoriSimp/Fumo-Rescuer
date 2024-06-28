using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WhipAttackScript : MonoBehaviour
{
    public Animator[] SpikeGroup;

    public float ActionDuration;
    public float DamageInstance;

    bool canDamagePlayer = false;
    bool damaged = false;

    private void Start()
    {
        SpikeGroup = GetComponentsInChildren<Animator>();

        StartCoroutine(Action());
    }

    void ActivateSpikes()
    {
        foreach (Animator obj in SpikeGroup)
        {
            obj.SetBool("active", true);
        }
    }

    public IEnumerator Action()
    {
        bool spikeActivated = false;
        float countup = 0;

        while (countup < ActionDuration)
        {
            if (countup >= (DamageInstance - 0.25f) && !spikeActivated)
            {
                ActivateSpikes(); 
                spikeActivated = true;
            }

            canDamagePlayer = countup >= DamageInstance && countup <= DamageInstance + 0.3f;

            yield return null;
            countup += Time.deltaTime;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!canDamagePlayer) return;

        if (collision && collision.gameObject.CompareTag("Player") && !damaged)
        {
            EnemyBehavior_EoE EoE = FindFirstObjectByType<EnemyBehavior_EoE>();
            if (EoE) EoE.DealDamage((int) (EoE.attackDamage * 1.5f), collision.gameObject.GetComponent<PlayerBehaviorScript>());
            damaged = true;
        }
    }
}
