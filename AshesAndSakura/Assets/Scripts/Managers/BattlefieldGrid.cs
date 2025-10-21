using System.Collections.Generic;
using UnityEngine;

namespace AshesAndSakura.Managers
{
    /// <summary>
    /// Manages the battlefield grid with lanes for unit placement
    /// </summary>
    public class BattlefieldGrid : MonoBehaviour
    {
        public static BattlefieldGrid Instance { get; private set; }

        [Header("Grid Configuration")]
        [SerializeField] private int numberOfLanes = 4;
        [SerializeField] private float laneSpacing = 1.5f;
        [SerializeField] private float laneYStart = -2.5f;
        [SerializeField] private float gridXStart = -8f;
        [SerializeField] private float gridXEnd = 8f;

        [Header("Visual Settings")]
        [SerializeField] private Color laneColor = new Color(1f, 1f, 1f, 0.2f);
        [SerializeField] private Color placementZoneColor = new Color(0f, 1f, 0f, 0.3f);
        [SerializeField] private float placementZoneWidth = 5f;

        private List<GridCell>[,] grid;
        private int gridWidth = 16; // Number of cells horizontally
        private float cellWidth;

        public int NumberOfLanes => numberOfLanes;

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

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            cellWidth = (gridXEnd - gridXStart) / gridWidth;
            grid = new List<GridCell>[gridWidth, numberOfLanes];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < numberOfLanes; y++)
                {
                    grid[x, y] = new List<GridCell>();
                }
            }

            Debug.Log($"[BattlefieldGrid] Initialized {gridWidth}x{numberOfLanes} grid");
        }

        /// <summary>
        /// Get the world position for a specific lane
        /// </summary>
        public Vector3 GetLanePosition(int laneIndex, float xPosition)
        {
            float yPos = laneYStart + (laneIndex * laneSpacing);
            return new Vector3(xPosition, yPos, 0f);
        }

        /// <summary>
        /// Get the Y position of a lane
        /// </summary>
        public float GetLaneYPosition(int laneIndex)
        {
            return laneYStart + (laneIndex * laneSpacing);
        }

        /// <summary>
        /// Snap a world position to the nearest valid grid position
        /// </summary>
        public (Vector3 position, int laneIndex) SnapToGrid(Vector3 worldPosition)
        {
            // Find nearest lane
            int lane = GetNearestLane(worldPosition.y);

            // Clamp X position to placement zone
            float clampedX = Mathf.Clamp(worldPosition.x, gridXStart, gridXStart + placementZoneWidth);

            Vector3 snappedPos = new Vector3(clampedX, GetLaneYPosition(lane), 0f);
            return (snappedPos, lane);
        }

        /// <summary>
        /// Get the nearest lane index based on Y position
        /// </summary>
        public int GetNearestLane(float yPosition)
        {
            int nearestLane = 0;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < numberOfLanes; i++)
            {
                float laneY = GetLaneYPosition(i);
                float distance = Mathf.Abs(yPosition - laneY);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestLane = i;
                }
            }

            return nearestLane;
        }

        /// <summary>
        /// Check if a position is within the placement zone
        /// </summary>
        public bool IsInPlacementZone(Vector3 worldPosition)
        {
            return worldPosition.x >= gridXStart &&
                   worldPosition.x <= gridXStart + placementZoneWidth;
        }

        /// <summary>
        /// Check if a cell is occupied by a unit
        /// </summary>
        public bool IsCellOccupied(Vector3 position, int laneIndex)
        {
            // Simple overlap check - can be expanded with proper grid tracking
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.5f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Unit"))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            // Draw lanes
            Gizmos.color = laneColor;
            float drawXStart = gridXStart;
            float drawXEnd = gridXEnd;

            for (int i = 0; i < numberOfLanes; i++)
            {
                float y = laneYStart + (i * laneSpacing);
                Gizmos.DrawLine(new Vector3(drawXStart, y, 0), new Vector3(drawXEnd, y, 0));
            }

            // Draw placement zone
            Gizmos.color = placementZoneColor;
            float placementX = drawXStart + (placementZoneWidth / 2f);
            float placementHeight = (numberOfLanes - 1) * laneSpacing;
            Gizmos.DrawCube(
                new Vector3(placementX, laneYStart + placementHeight / 2f, 0),
                new Vector3(placementZoneWidth, placementHeight + laneSpacing, 0.1f)
            );

            // Draw grid boundaries
            Gizmos.color = Color.white;
            float topY = laneYStart + (numberOfLanes - 1) * laneSpacing + laneSpacing / 2f;
            float bottomY = laneYStart - laneSpacing / 2f;

            // Left boundary
            Gizmos.DrawLine(new Vector3(drawXStart, bottomY, 0), new Vector3(drawXStart, topY, 0));
            // Right boundary
            Gizmos.DrawLine(new Vector3(drawXEnd, bottomY, 0), new Vector3(drawXEnd, topY, 0));
            // Top boundary
            Gizmos.DrawLine(new Vector3(drawXStart, topY, 0), new Vector3(drawXEnd, topY, 0));
            // Bottom boundary
            Gizmos.DrawLine(new Vector3(drawXStart, bottomY, 0), new Vector3(drawXEnd, bottomY, 0));
        }
    }

    public class GridCell
    {
        public Vector3 position;
        public bool isOccupied;
        public Units.Unit occupyingUnit;
    }
}
