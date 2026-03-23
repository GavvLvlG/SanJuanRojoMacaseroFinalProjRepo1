using UnityEngine;
using System.Collections; // Required for Coroutines

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Reference to the enemy Prefab
    public float spawnInterval = 3f; // Time between spawns
    public float spawnDelay = 1f; // Initial delay before spawning starts
    public float minX, maxX, minY, maxY; // Spawn area boundaries

    void Start()
    {
        // Start the spawning routine
        StartCoroutine(SpawnEnemiesRoutine()); 
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        // Wait for the initial delay
        yield return new WaitForSeconds(spawnDelay); 

        while (true) // Infinite loop to keep spawning
        {
            SpawnEnemy();
            // Wait for the specified interval before the next spawn
            yield return new WaitForSeconds(spawnInterval); 
        }
    }

    private void SpawnEnemy()
    {
        // Calculate a random position within the defined bounds
        Vector2 spawnPosition = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );

        // Instantiate the enemy prefab at the random position with no rotation
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}