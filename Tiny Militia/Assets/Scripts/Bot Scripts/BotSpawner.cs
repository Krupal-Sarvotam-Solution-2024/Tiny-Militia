using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public GameObject botPrefab; // Prefab for the bot
    public float spawnInterval = 5f; // Interval between bot spawns
    public Transform[] spawnPoints; // Array of spawn points for bots

    private void Start()
    {
        // Start spawning bots
        StartCoroutine(SpawnBots());
    }

    IEnumerator SpawnBots()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Spawn a bot at a random spawn point
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];
            Instantiate(botPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
