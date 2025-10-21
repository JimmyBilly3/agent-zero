using System;
using UnityEngine;
using AshesAndSakura.Data;
using AshesAndSakura.Managers;

namespace AshesAndSakura.Units
{
    /// <summary>
    /// Base class for all player units
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Unit : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform firePoint;

        private UnitData data;
        private int currentHealth;
        private float attackTimer;
        private int laneIndex;
        private Enemies.Enemy currentTarget;

        public event Action<Unit> OnDeath;
        public event Action<int, int> OnHealthChanged; // current, max

        public UnitData Data => data;
        public int CurrentHealth => currentHealth;
        public int LaneIndex => laneIndex;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (data == null) return;

            UpdateCombat();
        }

        /// <summary>
        /// Initialize unit with data
        /// </summary>
        public void Initialize(UnitData unitData, int lane)
        {
            data = unitData;
            laneIndex = lane;
            currentHealth = data.maxHealth;
            attackTimer = 0f;

            // Set visual
            if (spriteRenderer != null)
            {
                spriteRenderer.color = data.unitColor;
            }

            gameObject.name = $"Unit_{data.unitName}_{lane}";
            gameObject.tag = "Unit";

            Debug.Log($"[Unit] {data.unitName} initialized at lane {lane} with {currentHealth} HP");
        }

        private void UpdateCombat()
        {
            // Update attack cooldown
            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime;
                return;
            }

            // Find target if we don't have one
            if (currentTarget == null || !currentTarget.gameObject.activeSelf)
            {
                FindTarget();
            }

            // Attack target if in range
            if (currentTarget != null && IsTargetInRange())
            {
                Attack();
            }
        }

        private void FindTarget()
        {
            currentTarget = null;
            float closestDistance = float.MaxValue;

            // Find closest enemy in the same lane and within range
            foreach (var enemy in EnemySpawner.Instance.ActiveEnemies)
            {
                if (enemy.LaneIndex != laneIndex) continue;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= data.attackRange && distance < closestDistance)
                {
                    closestDistance = distance;
                    currentTarget = enemy;
                }
            }
        }

        private bool IsTargetInRange()
        {
            if (currentTarget == null) return false;
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            return distance <= data.attackRange;
        }

        private void Attack()
        {
            attackTimer = data.attackCooldown;

            if (currentTarget != null)
            {
                currentTarget.TakeDamage(data.attackDamage);
                Debug.Log($"[Unit] {data.unitName} attacked for {data.attackDamage} damage!");
            }
        }

        /// <summary>
        /// Take damage from enemy
        /// </summary>
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            OnHealthChanged?.Invoke(currentHealth, data.maxHealth);

            Debug.Log($"[Unit] {data.unitName} took {damage} damage. HP: {currentHealth}/{data.maxHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Heal the unit
        /// </summary>
        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(currentHealth + amount, data.maxHealth);
            OnHealthChanged?.Invoke(currentHealth, data.maxHealth);
            Debug.Log($"[Unit] {data.unitName} healed for {amount}. HP: {currentHealth}/{data.maxHealth}");
        }

        private void Die()
        {
            Debug.Log($"[Unit] {data.unitName} has been defeated!");
            OnDeath?.Invoke(this);

            if (UnitManager.Instance != null)
            {
                UnitManager.Instance.RemoveUnit(this);
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            if (data == null) return;

            // Draw attack range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.attackRange);
        }
    }
}
