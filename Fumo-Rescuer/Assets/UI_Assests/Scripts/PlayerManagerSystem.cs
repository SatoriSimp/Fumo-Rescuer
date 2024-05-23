using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayerManagementSystem : MonoBehaviour
{
    public Camera mainCamera;

    public PlayerBehaviorScript currentPlayer;

    public GameObject playerMelee_Prefab;
    public GameObject playerCaster_Prefab;

    public GameObject playerMelee_SwitchEffect;
    public GameObject playerCaster_SwitchEffect;

    public GameObject playerStatusBar;

    public bool isCurrentPlayerMelee = true; 

    public float playerSwitchCooldown = 20f;
    public float timeSinceLastPlayerSwitch = 0f;

    private void Start()
    {
        playerStatusBar.GetComponent<PlayerStatusesScript>().OnSystemStart_UpdatePlayerStatus(isCurrentPlayerMelee);
    }

    private void Update()
    {
        if (!currentPlayer || currentPlayer.currentHealth <= 0) return;

        timeSinceLastPlayerSwitch += Time.deltaTime;

        if (timeSinceLastPlayerSwitch >= playerSwitchCooldown)
        {
            playerStatusBar.GetComponent<PlayerStatusesScript>().OnSwitchReady_UpdatePlayerStatus(isCurrentPlayerMelee);
        }

        if (Input.GetKeyDown(KeyCode.Space) && timeSinceLastPlayerSwitch >= playerSwitchCooldown)
        {
            if (isCurrentPlayerMelee)
                Instantiate(playerCaster_SwitchEffect, currentPlayer.transform.position, Quaternion.identity);
            else 
                Instantiate(playerMelee_SwitchEffect, currentPlayer.transform.position, Quaternion.identity);
            SwitchPlayerUnit();
            playerStatusBar.GetComponent<PlayerStatusesScript>().OnPlayerChange_UpdatePlayerStatus(isCurrentPlayerMelee);
        }
    }

    public void SwitchPlayerUnit()
    {
        int CurrentPlayer_HealthPercentage = currentPlayer.currentHealth * 100 / currentPlayer.maxHealth;
        GameObject Prefab_NewPlayer;
        if (isCurrentPlayerMelee) 
        {
            Prefab_NewPlayer = Instantiate(playerCaster_Prefab, currentPlayer.transform.position, Quaternion.identity);
        }
        else
        {
            Prefab_NewPlayer = Instantiate(playerMelee_Prefab, currentPlayer.transform.position, Quaternion.identity);
        }

        PlayerBehaviorScript NewPlayer = Prefab_NewPlayer.GetComponent<PlayerBehaviorScript>();

        NewPlayer.Start();

        NewPlayer.currentHealth = NewPlayer.maxHealth * CurrentPlayer_HealthPercentage / 100;
        NewPlayer.healthBar.setHealth(NewPlayer.currentHealth);
        NewPlayer.invulnerable = 2f;

        StartCoroutine(mainCamera.GetComponent<CameraMovement>()
            .SwitchLockingIntoNewPlayer(Prefab_NewPlayer.GetComponent<PlayerBehaviorScript>()));

        Destroy(currentPlayer.gameObject);
        currentPlayer = Prefab_NewPlayer.GetComponent<PlayerBehaviorScript>();

        timeSinceLastPlayerSwitch = 0f;
        isCurrentPlayerMelee = !isCurrentPlayerMelee;
    }
}
