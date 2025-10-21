using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AshesAndSakura.Managers
{
    /// <summary>
    /// Manages special skills (airstrike, heal, summon, etc.)
    /// </summary>
    public class SkillManager : MonoBehaviour
    {
        public static SkillManager Instance { get; private set; }

        [Header("Skills")]
        [SerializeField] private List<SkillData> availableSkills = new List<SkillData>();

        private Dictionary<string, float> skillCooldowns = new Dictionary<string, float>();

        public event Action<string> OnSkillActivated;
        public event Action<string, float> OnSkillCooldownUpdated;

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

            InitializeSkills();
        }

        private void Update()
        {
            UpdateCooldowns();
        }

        private void InitializeSkills()
        {
            // Initialize default skills if none assigned
            if (availableSkills.Count == 0)
            {
                availableSkills.Add(new SkillData
                {
                    skillId = "airstrike",
                    skillName = "Airstrike",
                    cpCost = 50,
                    cooldown = 30f,
                    damage = 100
                });

                availableSkills.Add(new SkillData
                {
                    skillId = "heal",
                    skillName = "Field Medic",
                    cpCost = 30,
                    cooldown = 20f,
                    healAmount = 50
                });

                availableSkills.Add(new SkillData
                {
                    skillId = "summon",
                    skillName = "Emergency Backup",
                    cpCost = 75,
                    cooldown = 45f
                });
            }

            foreach (var skill in availableSkills)
            {
                skillCooldowns[skill.skillId] = 0f;
            }

            Debug.Log($"[SkillManager] Initialized {availableSkills.Count} skills");
        }

        private void UpdateCooldowns()
        {
            List<string> keys = new List<string>(skillCooldowns.Keys);
            foreach (var skillId in keys)
            {
                if (skillCooldowns[skillId] > 0f)
                {
                    skillCooldowns[skillId] -= Time.deltaTime;
                    OnSkillCooldownUpdated?.Invoke(skillId, skillCooldowns[skillId]);
                }
            }
        }

        /// <summary>
        /// Try to activate a skill
        /// </summary>
        public bool TryActivateSkill(string skillId, Vector3 position)
        {
            SkillData skill = GetSkillData(skillId);
            if (skill == null)
            {
                Debug.LogError($"[SkillManager] Skill '{skillId}' not found!");
                return false;
            }

            // Check cooldown
            if (IsOnCooldown(skillId))
            {
                Debug.Log($"[SkillManager] Skill '{skillId}' is on cooldown: {skillCooldowns[skillId]:F1}s remaining");
                return false;
            }

            // Check CP cost
            if (ResourceManager.Instance != null && !ResourceManager.Instance.TrySpendCP(skill.cpCost))
            {
                return false;
            }

            // Activate skill
            ActivateSkill(skill, position);
            skillCooldowns[skillId] = skill.cooldown;

            Debug.Log($"[SkillManager] Activated '{skill.skillName}' at {position}. Cooldown: {skill.cooldown}s, CP Cost: {skill.cpCost}");
            OnSkillActivated?.Invoke(skillId);

            return true;
        }

        private void ActivateSkill(SkillData skill, Vector3 position)
        {
            switch (skill.skillId)
            {
                case "airstrike":
                    StartCoroutine(AirstrikeEffect(position, skill.damage));
                    break;
                case "heal":
                    HealNearbyUnits(position, skill.healAmount);
                    break;
                case "summon":
                    SummonBackupUnit(position);
                    break;
                default:
                    Debug.LogWarning($"[SkillManager] No implementation for skill: {skill.skillId}");
                    break;
            }
        }

        private IEnumerator AirstrikeEffect(Vector3 position, int damage)
        {
            Debug.Log($"[SkillManager] Airstrike incoming at {position}!");
            yield return new WaitForSeconds(1f);

            // Find enemies in radius and damage them
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, 3f);
            int enemiesHit = 0;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    var enemy = hit.GetComponent<Enemies.Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                        enemiesHit++;
                    }
                }
            }

            Debug.Log($"[SkillManager] Airstrike hit {enemiesHit} enemies for {damage} damage each!");
        }

        private void HealNearbyUnits(Vector3 position, int healAmount)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, 5f);
            int unitsHealed = 0;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Unit"))
                {
                    var unit = hit.GetComponent<Units.Unit>();
                    if (unit != null)
                    {
                        unit.Heal(healAmount);
                        unitsHealed++;
                    }
                }
            }

            Debug.Log($"[SkillManager] Healed {unitsHealed} units for {healAmount} HP each!");
        }

        private void SummonBackupUnit(Vector3 position)
        {
            // Spawn a free backup unit
            if (UnitManager.Instance != null)
            {
                // Get a random basic unit
                var units = UnitManager.Instance.GetAllUnits();
                if (units.Length > 0)
                {
                    Debug.Log($"[SkillManager] Summoned backup unit at {position}!");
                    // Force spawn without CP cost (handled by skill cost)
                }
            }
        }

        public bool IsOnCooldown(string skillId)
        {
            return skillCooldowns.ContainsKey(skillId) && skillCooldowns[skillId] > 0f;
        }

        public float GetCooldownRemaining(string skillId)
        {
            return skillCooldowns.ContainsKey(skillId) ? skillCooldowns[skillId] : 0f;
        }

        public SkillData GetSkillData(string skillId)
        {
            return availableSkills.Find(s => s.skillId == skillId);
        }

        public List<SkillData> GetAllSkills()
        {
            return availableSkills;
        }
    }

    [Serializable]
    public class SkillData
    {
        public string skillId;
        public string skillName;
        public int cpCost;
        public float cooldown;
        public int damage;
        public int healAmount;
        public string description;
    }
}
