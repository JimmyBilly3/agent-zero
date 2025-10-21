using UnityEngine;
using UnityEngine.EventSystems;
using AshesAndSakura.Managers;
using AshesAndSakura.Data;

namespace AshesAndSakura.UI
{
    /// <summary>
    /// Handles drag and drop of units from UI to battlefield
    /// </summary>
    public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Unit Settings")]
        [SerializeField] private string unitId;
        [SerializeField] private UnitData unitData;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject dragPreviewPrefab;
        [SerializeField] private Color canPlaceColor = Color.green;
        [SerializeField] private Color cannotPlaceColor = Color.red;

        private GameObject dragPreview;
        private Camera mainCamera;
        private bool isDragging = false;
        private Vector3 dragStartPosition;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public void SetUnitData(string id, UnitData data)
        {
            unitId = id;
            unitData = data;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (unitData == null)
            {
                Debug.LogWarning("[DragDropHandler] No unit data assigned!");
                return;
            }

            // Check if player has enough CP
            if (ResourceManager.Instance != null && !ResourceManager.Instance.HasEnoughCP(unitData.cpCost))
            {
                Debug.Log($"[DragDropHandler] Not enough CP for {unitData.unitName}");
                return;
            }

            isDragging = true;
            dragStartPosition = transform.position;

            // Create drag preview
            if (dragPreviewPrefab != null)
            {
                dragPreview = Instantiate(dragPreviewPrefab);
                dragPreview.transform.localScale = Vector3.one * 0.8f;
            }
            else
            {
                // Create simple preview
                dragPreview = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                dragPreview.transform.localScale = Vector3.one * 0.5f;
                Destroy(dragPreview.GetComponent<Collider>());

                var renderer = dragPreview.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = unitData.unitColor;
                }
            }

            Debug.Log($"[DragDropHandler] Started dragging {unitData.unitName}");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging || dragPreview == null) return;

            // Convert screen position to world position
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0f;

            dragPreview.transform.position = worldPos;

            // Update preview color based on placement validity
            if (BattlefieldGrid.Instance != null)
            {
                bool canPlace = BattlefieldGrid.Instance.IsInPlacementZone(worldPos);
                var renderer = dragPreview.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = canPlace ? canPlaceColor : cannotPlaceColor;
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            isDragging = false;

            // Convert screen position to world position
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0f;

            // Try to place unit
            bool placed = TryPlaceUnit(worldPos);

            // Cleanup preview
            if (dragPreview != null)
            {
                Destroy(dragPreview);
            }

            if (placed)
            {
                Debug.Log($"[DragDropHandler] Successfully placed {unitData.unitName}");
            }
            else
            {
                Debug.Log($"[DragDropHandler] Failed to place {unitData.unitName}");
            }
        }

        private bool TryPlaceUnit(Vector3 worldPosition)
        {
            if (BattlefieldGrid.Instance == null || UnitManager.Instance == null)
            {
                Debug.LogError("[DragDropHandler] Required managers not found!");
                return false;
            }

            // Check if in placement zone
            if (!BattlefieldGrid.Instance.IsInPlacementZone(worldPosition))
            {
                Debug.Log("[DragDropHandler] Position outside placement zone");
                return false;
            }

            // Snap to grid
            var (snappedPos, laneIndex) = BattlefieldGrid.Instance.SnapToGrid(worldPosition);

            // Check if cell is occupied
            if (BattlefieldGrid.Instance.IsCellOccupied(snappedPos, laneIndex))
            {
                Debug.Log("[DragDropHandler] Position already occupied");
                return false;
            }

            // Try to place unit through manager
            return UnitManager.Instance.TryPlaceUnit(unitId, snappedPos, laneIndex);
        }
    }
}
