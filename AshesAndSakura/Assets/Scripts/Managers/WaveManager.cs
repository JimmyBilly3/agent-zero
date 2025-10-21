using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshesAndSakura.Managers
{
    /// <summary>
    /// Manages wave-based enemy spawning with increasing difficulty
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }

        [Header("Wave Settings")]
        [SerializeField] private float timeBetweenWaves = 10f;
        [SerializeField] private float waveStartDelay = 5f;
        [SerializeField] private int startingWave = 1;

        [Header("Difficulty Scaling")]
        [SerializeField] private float enemyHealthMultiplier = 1.1f;
        [SerializeField] private float enemyDamageMultiplier = 1.05f;
        [SerializeField] private int baseEnemiesPerWave = 5;
        [SerializeField] private int enemiesIncreasePerWave = 2;

        [Header("Random Events")]
        [SerializeField] private int eventCheckInterval = 3; // Check every 3 waves
        [SerializeField] private float eventChance = 0.5f; // 50% chance

        private int currentWave = 0;
        private bool waveInProgress = false;
        private float waveTimer = 0f;
        private int enemiesRemainingInWave = 0;

        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveCompleted;
        public event Action<string> OnRandomEvent;

        public int CurrentWave => currentWave;
        public bool WaveInProgress => waveInProgress;
        public int EnemiesRemaining => enemiesRemainingInWave;

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
        }

        private void Start()
        {
            currentWave = startingWave - 1;
            StartCoroutine(WaveRoutine());
        }

        private IEnumerator WaveRoutine()
        {
            yield return new WaitForSeconds(waveStartDelay);

            while (true)
            {
                currentWave++;
                StartWave();

                // Wait until all enemies are defeated
                yield return new WaitUntil(() => enemiesRemainingInWave <= 0);

                CompleteWave();

                // Check for random events
                if (currentWave % eventCheckInterval == 0)
                {
                    TriggerRandomEvent();
                }

                // Wait between waves
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        private void StartWave()
        {
            waveInProgress = true;
            int enemyCount = CalculateEnemyCount();
            enemiesRemainingInWave = enemyCount;

            Debug.Log($"[WaveManager] Wave {currentWave} started! Enemies: {enemyCount}");
            OnWaveStarted?.Invoke(currentWave);

            // Notify EnemySpawner to spawn enemies
            if (EnemySpawner.Instance != null)
            {
                EnemySpawner.Instance.SpawnWave(currentWave, enemyCount, GetDifficultyMultipliers());
            }
        }

        private void CompleteWave()
        {
            waveInProgress = false;
            Debug.Log($"[WaveManager] Wave {currentWave} completed!");
            OnWaveCompleted?.Invoke(currentWave);

            // Reward player with CP
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnWaveCompleted();
            }
        }

        private int CalculateEnemyCount()
        {
            return baseEnemiesPerWave + (currentWave - 1) * enemiesIncreasePerWave;
        }

        public (float health, float damage) GetDifficultyMultipliers()
        {
            float healthMult = Mathf.Pow(enemyHealthMultiplier, currentWave - 1);
            float damageMult = Mathf.Pow(enemyDamageMultiplier, currentWave - 1);
            return (healthMult, damageMult);
        }

        public void OnEnemyDefeated()
        {
            enemiesRemainingInWave--;
            Debug.Log($"[WaveManager] Enemy defeated. Remaining: {enemiesRemainingInWave}");
        }

        private void TriggerRandomEvent()
        {
            if (UnityEngine.Random.value > eventChance) return;

            string[] events = new string[]
            {
                "Storm",
                "BossWave",
                "DoubleSpawn",
                "FastEnemies",
                "ArmoredEnemies"
            };

            string selectedEvent = events[UnityEngine.Random.Range(0, events.Length)];
            Debug.Log($"[WaveManager] Random Event: {selectedEvent}");
            OnRandomEvent?.Invoke(selectedEvent);
        }

        /// <summary>
        /// Force start next wave (skip waiting time)
        /// </summary>
        public void ForceNextWave()
        {
            if (!waveInProgress)
            {
                StopAllCoroutines();
                StartCoroutine(WaveRoutine());
            }
        }
    }
}
