using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshesAndSakura.Managers
{
    /// <summary>
    /// Handles enemy spawning from the right side of the screen with randomized placement
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance { get; private set; }

        [Header("Spawn Settings")]
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private Transform spawnParent;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private Vector2 spawnAreaMin = new Vector2(10f, -3f);
        [SerializeField] private Vector2 spawnAreaMax = new Vector2(12f, 3f);

        [Header("Lane Configuration")]
        [SerializeField] private int numberOfLanes = 4;
        [SerializeField] private float laneSpacing = 1.5f;
        [SerializeField] private float laneYStart = -2.5f;

        private List<Enemies.Enemy> activeEnemies = new List<Enemies.Enemy>();

        public List<Enemies.Enemy> ActiveEnemies => activeEnemies;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Create default enemy prefab if none assigned
            if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            {
                Debug.LogWarning("[EnemySpawner] No enemy prefabs assigned!");
            }
        }

        /// <summary>
        /// Spawn enemies for a wave
        /// </summary>
        public void SpawnWave(int waveNumber, int enemyCount, (float health, float damage) multipliers)
        {
            StartCoroutine(SpawnWaveRoutine(waveNumber, enemyCount, multipliers));
        }

        private IEnumerator SpawnWaveRoutine(int waveNumber, int enemyCount, (float health, float damage) multipliers)
        {
            Debug.Log($"[EnemySpawner] Spawning {enemyCount} enemies for wave {waveNumber}");

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnRandomEnemy(multipliers);
                yield return new WaitForSeconds(spawnInterval);
            }

            Debug.Log($"[EnemySpawner] Finished spawning wave {waveNumber}");
        }

        private void SpawnRandomEnemy((float health, float damage) multipliers)
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            {
                Debug.LogError("[EnemySpawner] Cannot spawn enemy - no prefabs!");
                return;
            }

            // Random spawn position
            int randomLane = Random.Range(0, numberOfLanes);
            float yPos = laneYStart + (randomLane * laneSpacing);
            float xPos = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            Vector3 spawnPos = new Vector3(xPos, yPos, 0f);

            // Select random enemy type
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, spawnParent);

            Enemies.Enemy enemy = enemyObj.GetComponent<Enemies.Enemy>();
            if (enemy != null)
            {
                enemy.Initialize(randomLane, multipliers.health, multipliers.damage);
                enemy.OnDeath += HandleEnemyDeath;
                activeEnemies.Add(enemy);

                Debug.Log($"[EnemySpawner] Spawned enemy at lane {randomLane}, pos {spawnPos}");
            }
            else
            {
                Debug.LogError("[EnemySpawner] Enemy prefab missing Enemy component!");
                Destroy(enemyObj);
            }
        }

        private void HandleEnemyDeath(Enemies.Enemy enemy)
        {
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }

            enemy.OnDeath -= HandleEnemyDeath;

            // Notify wave manager
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnEnemyDefeated();
            }

            Debug.Log($"[EnemySpawner] Enemy defeated. Remaining: {activeEnemies.Count}");
        }

        /// <summary>
        /// Get Y position for a specific lane
        /// </summary>
        public float GetLaneYPosition(int laneIndex)
        {
            return laneYStart + (laneIndex * laneSpacing);
        }

        /// <summary>
        /// Clear all active enemies
        /// </summary>
        public void ClearAllEnemies()
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    enemy.OnDeath -= HandleEnemyDeath;
                    Destroy(enemy.gameObject);
                }
            }
            activeEnemies.Clear();
            Debug.Log("[EnemySpawner] All enemies cleared");
        }
    }
}
