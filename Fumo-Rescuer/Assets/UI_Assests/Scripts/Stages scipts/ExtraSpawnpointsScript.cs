using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraSpawnpointsScript : MonoBehaviour
{
    [SerializeField] private GameObject[] Extra_Spawnpoints;
    [SerializeField] private GameObject[] Remove_Spawnpoints;

    private void Start()
    {
        int difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        if (difficulty < 3)
        {
            foreach (GameObject Extra_Spawnpoint in Extra_Spawnpoints)
            {
                Destroy(Extra_Spawnpoint);
            }
        }
        else
        {
            foreach (GameObject gameObject in Remove_Spawnpoints)
            {
                Destroy(gameObject);
            }
        }
    }
}
