using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispSpawnpoint : MonoBehaviour
{
    public GameObject WispPrefab;
    public Transform EndPostion;
    public float WispMoveTime = 7;
    public float SpawnInterval;
    [SerializeField] private float SpawnIntervalCount = 0;

    private void Update()
    {
        SpawnIntervalCount += Time.deltaTime;
        if (SpawnIntervalCount >= SpawnInterval)
        {
            StartCoroutine(SpawnWisp());
        }
    }

    IEnumerator SpawnWisp()
    {
        SpawnIntervalCount = 0;
        GameObject wisp = Instantiate(WispPrefab, transform.position, Quaternion.identity);
        wisp.GetComponent<EnemyBehavior_Wisp>().endPosition = EndPostion.position;
        wisp.GetComponent<EnemyBehavior_Wisp>().moveTime = WispMoveTime;
        wisp.GetComponent<EnemyBehavior_Wisp>().StartMoving();
        yield return new WaitForEndOfFrame();
    }
}
