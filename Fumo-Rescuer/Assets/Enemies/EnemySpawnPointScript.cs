using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnPointScript : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    void SpawnPrefabEnemyAndSelfDestruc()
    {
        Debug.Log("New enemy spawned!");
        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
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
