using UnityEngine;

public class EnemySpawnPointScript : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public bool displayTooltips = false;
    public short spawnNumber = 1;

    private PlayerBehaviorScript presentingPlayerUnit;
    public bool targetPlayerUponSpawned = false;

    void SpawnPrefabEnemyAndSelfDestruc()
    {
        Debug.Log("New enemy spawned!");

        UnityEngine.Vector3 SpawnPosition = spawnPoint.position; 

        for (short i = 0; i < spawnNumber; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, SpawnPosition, Quaternion.identity);
            if (targetPlayerUponSpawned)
            {
                presentingPlayerUnit = FindFirstObjectByType<PlayerBehaviorScript>();
                if (presentingPlayerUnit) enemy.GetComponent<EnemyBehaviorScript>().playerDetected = presentingPlayerUnit.transform;
            }
            SpawnPosition += new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
            new WaitForEndOfFrame();
        }

        if (displayTooltips) enemyPrefab.GetComponent<EnemyBehaviorScript>().DisplayTooltips();
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
