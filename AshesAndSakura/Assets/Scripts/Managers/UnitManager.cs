using System.Collections.Generic;
using UnityEngine;
using AshesAndSakura.Units;
using AshesAndSakura.Data;

namespace AshesAndSakura.Managers
{
    /// <summary>
    /// Manages unit data, placement, and tracking
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance { get; private set; }

        [Header("Unit Database")]
        [SerializeField] private TextAsset unitDataJson;
        [SerializeField] private GameObject unitPrefab;

        [Header("Placement Settings")]
        [SerializeField] private LayerMask placementLayer;
        [SerializeField] private float placementCooldown = 0.5f;

        private UnitDatabase unitDatabase;
        private List<Unit> activeUnits = new List<Unit>();
        private float lastPlacementTime = 0f;
        private Unit currentDraggedUnit = null;

        public List<Unit> ActiveUnits => activeUnits;
        public UnitDatabase Database => unitDatabase;

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

            LoadUnitData();
        }

        private void LoadUnitData()
        {
            if (unitDataJson != null)
            {
                unitDatabase = JsonUtility.FromJson<UnitDatabase>(unitDataJson.text);
                Debug.Log($"[UnitManager] Loaded {unitDatabase.units.Length} units from database");
            }
            else
            {
                Debug.LogError("[UnitManager] Unit data JSON not assigned!");
                unitDatabase = new UnitDatabase();
            }
        }

        /// <summary>
        /// Attempt to place a unit at the specified position
        /// </summary>
        public bool TryPlaceUnit(string unitId, Vector3 position, int laneIndex)
        {
            // Check cooldown
            if (Time.time - lastPlacementTime < placementCooldown)
            {
                Debug.Log("[UnitManager] Placement on cooldown");
                return false;
            }

            // Get unit data
            UnitData data = GetUnitData(unitId);
            if (data == null)
            {
                Debug.LogError($"[UnitManager] Unit '{unitId}' not found in database");
                return false;
            }

            // Check if player has enough CP
            if (ResourceManager.Instance != null && !ResourceManager.Instance.TrySpendCP(data.cpCost))
            {
                return false;
            }

            // Create unit instance
            GameObject unitObj = Instantiate(unitPrefab, position, Quaternion.identity);
            Unit unit = unitObj.GetComponent<Unit>();

            if (unit != null)
            {
                unit.Initialize(data, laneIndex);
                activeUnits.Add(unit);
                lastPlacementTime = Time.time;

                Debug.Log($"[UnitManager] Placed {data.unitName} at lane {laneIndex}, cost {data.cpCost} CP");
                return true;
            }

            Destroy(unitObj);
            return false;
        }

        /// <summary>
        /// Remove a unit from active tracking
        /// </summary>
        public void RemoveUnit(Unit unit)
        {
            if (activeUnits.Contains(unit))
            {
                activeUnits.Remove(unit);
                Debug.Log($"[UnitManager] Removed unit. Active units: {activeUnits.Count}");
            }
        }

        /// <summary>
        /// Get unit data by ID
        /// </summary>
        public UnitData GetUnitData(string unitId)
        {
            if (unitDatabase == null || unitDatabase.units == null) return null;

            foreach (var data in unitDatabase.units)
            {
                if (data.unitId == unitId)
                    return data;
            }

            return null;
        }

        /// <summary>
        /// Get all available units for UI display
        /// </summary>
        public UnitData[] GetAllUnits()
        {
            return unitDatabase?.units ?? new UnitData[0];
        }

        /// <summary>
        /// Clear all active units (for game restart)
        /// </summary>
        public void ClearAllUnits()
        {
            foreach (var unit in activeUnits)
            {
                if (unit != null)
                    Destroy(unit.gameObject);
            }
            activeUnits.Clear();
            Debug.Log("[UnitManager] All units cleared");
        }

        public void SetDraggedUnit(Unit unit)
        {
            currentDraggedUnit = unit;
        }

        public Unit GetDraggedUnit()
        {
            return currentDraggedUnit;
        }
    }
}
