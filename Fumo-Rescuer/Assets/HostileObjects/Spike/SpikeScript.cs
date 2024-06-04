using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private float Uptime = 2;
    [SerializeField] private float Downtime = 4;

    private float ActiveTimeCount = 0;
    [SerializeField] private bool IsActive = false;
    private static bool IsPlayerDamagedInCurrentCycle = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ActiveTimeCount += Time.deltaTime;
        if ((!IsActive && ActiveTimeCount >= Downtime) || (IsActive && ActiveTimeCount >= Uptime))
        {
            ChangeSpikeActivationStatus();
        }
    }

    void ChangeSpikeActivationStatus()
    {
        ActiveTimeCount = 0;
        IsPlayerDamagedInCurrentCycle = false;
        IsActive = !IsActive;
        animator.SetBool("active", IsActive);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && IsActive && ActiveTimeCount > .5f && !IsPlayerDamagedInCurrentCycle)
        {
            IsPlayerDamagedInCurrentCycle = true;
            collision.GetComponent<PlayerBehaviorScript>().TakeDamage(0, 0, 50, null);
        }
    }
}
