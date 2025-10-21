using System;
using UnityEngine;

namespace AshesAndSakura.Data
{
    [Serializable]
    public class UnitData
    {
        public string unitId;
        public string unitName;
        public string description;

        [Header("Stats")]
        public int maxHealth;
        public int attackDamage;
        public float attackRange;
        public float attackCooldown;
        public float moveSpeed;

        [Header("Cost & Type")]
        public int cpCost;
        public UnitType unitType;
        public AttackType attackType;

        [Header("Visual")]
        public string spritePath; // Path to sprite asset
        public Color unitColor = Color.white;
    }

    [Serializable]
    public class UnitDatabase
    {
        public UnitData[] units;
    }

    public enum UnitType
    {
        Infantry,
        Tank,
        Sniper,
        Medic,
        Support
    }

    public enum AttackType
    {
        Melee,
        Ranged,
        Splash
    }
}
