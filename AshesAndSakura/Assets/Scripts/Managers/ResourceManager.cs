using System;
using UnityEngine;

namespace AshesAndSakura.Managers
{
    /// <summary>
    /// Manages Command Points (CP) - the resource used for placing units and activating skills
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [Header("Command Points Settings")]
        [SerializeField] private int startingCP = 100;
        [SerializeField] private int maxCP = 999;
        [SerializeField] private int cpPerSecond = 5;
        [SerializeField] private int cpPerWave = 50;

        private int currentCP;
        private float cpTimer = 0f;

        public event Action<int> OnCPChanged;
        public event Action<int, int> OnCPSpent; // amount, remaining

        public int CurrentCP => currentCP;
        public int MaxCP => maxCP;

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

            currentCP = startingCP;
        }

        private void Update()
        {
            // Passive CP generation
            cpTimer += Time.deltaTime;
            if (cpTimer >= 1f)
            {
                cpTimer = 0f;
                AddCP(cpPerSecond);
            }
        }

        /// <summary>
        /// Add CP to the pool
        /// </summary>
        public void AddCP(int amount)
        {
            currentCP = Mathf.Min(currentCP + amount, maxCP);
            Debug.Log($"[ResourceManager] Added {amount} CP. Total: {currentCP}/{maxCP}");
            OnCPChanged?.Invoke(currentCP);
        }

        /// <summary>
        /// Try to spend CP. Returns true if successful.
        /// </summary>
        public bool TrySpendCP(int amount)
        {
            if (currentCP >= amount)
            {
                currentCP -= amount;
                Debug.Log($"[ResourceManager] Spent {amount} CP. Remaining: {currentCP}/{maxCP}");
                OnCPSpent?.Invoke(amount, currentCP);
                OnCPChanged?.Invoke(currentCP);
                return true;
            }

            Debug.LogWarning($"[ResourceManager] Not enough CP! Required: {amount}, Available: {currentCP}");
            return false;
        }

        /// <summary>
        /// Called when a wave is completed
        /// </summary>
        public void OnWaveCompleted()
        {
            AddCP(cpPerWave);
        }

        /// <summary>
        /// Check if player has enough CP
        /// </summary>
        public bool HasEnoughCP(int amount)
        {
            return currentCP >= amount;
        }

        /// <summary>
        /// Reset CP to starting value (for game restart)
        /// </summary>
        public void ResetCP()
        {
            currentCP = startingCP;
            OnCPChanged?.Invoke(currentCP);
            Debug.Log($"[ResourceManager] CP Reset to {startingCP}");
        }
    }
}
