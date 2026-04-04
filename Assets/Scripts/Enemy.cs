using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Simplified Enemy spawner: small set of inspector fields and clear behavior wiring.
public class Enemy : MonoBehaviour
{
    [Header("Spawner")]
    public GameObject enemyPrefab;
    public float spawnDelay = 1f;
    public float spawnInterval = 3f;
    public float minX, maxX, minY, maxY;

    [Header("Spawn Control")]
    public bool allowMultipleSpawns = true; // when false, only one spawned enemy will exist at a time
    [Tooltip("0 = unlimited. When >0, the spawner will not create more than this many active enemies.")]
    public int maxActiveSpawns = 0;

    // Internal tracking of currently spawned instances. Null entries are cleaned up automatically.
    List<GameObject> activeSpawns = new List<GameObject>();

    [Header("Behavior defaults")]
    public float minMoveSpeed = 0.8f;
    public float maxMoveSpeed = 1.6f;
    public float wanderRadius = 2f;
    public float wanderInterval = 2f;

    [Header("Attack defaults")]
    [Range(0f, 1f)] public float chanceToAttack = 0.5f;
    public float minAttackRate = 1.5f;
    public float maxAttackRate = 3f;
    public float minAttackDamage = 5f;
    public float maxAttackDamage = 10f;
    public float attackRange = 1.5f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        // Clean up destroyed entries from tracking list
        activeSpawns.RemoveAll(item => item == null);

        // Respect allowMultipleSpawns flag and maxActiveSpawns limit
        if (!allowMultipleSpawns && activeSpawns.Count > 0)
        {
            // There's already an active spawned enemy; do not spawn another
            return;
        }

        if (maxActiveSpawns > 0 && activeSpawns.Count >= maxActiveSpawns)
        {
            // Reached limit of active spawned enemies
            return;
        }

        Vector2 pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    GameObject spawned = Instantiate(enemyPrefab, pos, Quaternion.identity);

    // Track spawn so we don't accidentally remove/destroy the initial one elsewhere
    activeSpawns.Add(spawned);

        // Ensure a child GameObject named "Behavior" holds the EnemyBehavior component (keeps hierarchy tidy)
        EnemyBehavior eb = null;
        Transform child = spawned.transform.Find("Behavior");
        if (child != null)
        {
            eb = child.GetComponent<EnemyBehavior>();
            if (eb == null) eb = child.gameObject.AddComponent<EnemyBehavior>();
        }
        else
        {
            GameObject go = new GameObject("Behavior");
            go.transform.SetParent(spawned.transform);
            go.transform.localPosition = Vector3.zero;
            eb = go.AddComponent<EnemyBehavior>();
        }

        // Initialize with simple randomized params
        float speed = Random.Range(minMoveSpeed, maxMoveSpeed);
        bool hasAttack = Random.value < chanceToAttack;
        float attackRate = hasAttack ? Random.Range(minAttackRate, maxAttackRate) : 0f;
        float attackDamage = hasAttack ? Random.Range(minAttackDamage, maxAttackDamage) : 0f;

        eb.InitializeSimple(speed, wanderRadius, wanderInterval, hasAttack, attackRate, attackDamage, attackRange);
    }
}