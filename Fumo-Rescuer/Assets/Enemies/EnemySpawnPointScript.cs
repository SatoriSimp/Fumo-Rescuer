using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnPointScript : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public bool displayTooltips = false;

    void SpawnPrefabEnemyAndSelfDestruc()
    {
        Debug.Log("New enemy spawned!");
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        if (displayTooltips) enemy.GetComponent<EnemyBehaviorScript>().DisplayTooltips();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SpawnPrefabEnemyAndSelfDestruc();
        }
    }
}
