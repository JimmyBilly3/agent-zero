using System;
using UnityEngine;

namespace AshesAndSakura.Enemies
{
    /// <summary>
    /// Base enemy class that moves from right to left and attacks units
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Stats")]
        [SerializeField] private int baseHealth = 50;
        [SerializeField] private int baseDamage = 10;
        [SerializeField] private float moveSpeed = 1.5f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 2f;

        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        private int currentHealth;
        private int currentDamage;
        private float attackTimer;
        private int laneIndex;
        private Units.Unit currentTarget;
        private bool isAlive = true;

        public event Action<Enemy> OnDeath;

        public int LaneIndex => laneIndex;
        public int CurrentHealth => currentHealth;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (!isAlive) return;

            UpdateMovement();
            UpdateCombat();
        }

        /// <summary>
        /// Initialize enemy with difficulty multipliers
        /// </summary>
        public void Initialize(int lane, float healthMultiplier, float damageMultiplier)
        {
            laneIndex = lane;
            currentHealth = Mathf.RoundToInt(baseHealth * healthMultiplier);
            currentDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
            attackTimer = 0f;
            isAlive = true;

            gameObject.tag = "Enemy";
            gameObject.name = $"Enemy_Lane{lane}";

            // Set color based on difficulty
            if (spriteRenderer != null)
            {
                float intensity = Mathf.Clamp01(healthMultiplier / 3f);
                spriteRenderer.color = Color.Lerp(Color.white, Color.red, intensity);
            }

            Debug.Log($"[Enemy] Spawned at lane {lane} with {currentHealth} HP and {currentDamage} DMG");
        }

        private void UpdateMovement()
        {
            // Stop moving if we have a target in range
            if (currentTarget != null && IsTargetInRange())
            {
                return;
            }

            // Move left towards player base
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            // Check if reached the end (player base)
            if (transform.position.x < -10f)
            {
                ReachPlayerBase();
            }
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

            // Find closest unit in the same lane ahead of us (to the left)
            if (Managers.UnitManager.Instance != null)
            {
                foreach (var unit in Managers.UnitManager.Instance.ActiveUnits)
                {
                    if (unit.LaneIndex != laneIndex) continue;
                    if (unit.transform.position.x > transform.position.x) continue; // Only targets ahead

                    float distance = Vector3.Distance(transform.position, unit.transform.position);
                    if (distance <= attackRange && distance < closestDistance)
                    {
                        closestDistance = distance;
                        currentTarget = unit;
                    }
                }
            }
        }

        private bool IsTargetInRange()
        {
            if (currentTarget == null) return false;
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            return distance <= attackRange;
        }

        private void Attack()
        {
            attackTimer = attackCooldown;

            if (currentTarget != null)
            {
                currentTarget.TakeDamage(currentDamage);
                Debug.Log($"[Enemy] Attacked unit for {currentDamage} damage!");
            }
        }

        /// <summary>
        /// Take damage from units or skills
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (!isAlive) return;

            currentHealth -= damage;
            Debug.Log($"[Enemy] Took {damage} damage. HP: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (!isAlive) return;
            isAlive = false;

            Debug.Log($"[Enemy] Defeated at lane {laneIndex}!");
            OnDeath?.Invoke(this);

            Destroy(gameObject, 0.1f);
        }

        private void ReachPlayerBase()
        {
            Debug.LogWarning("[Enemy] Reached player base! Game Over condition!");
            // TODO: Notify GameManager of base damage
            Die();
        }

        private void OnDrawGizmosSelected()
        {
            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
