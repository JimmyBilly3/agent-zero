using UnityEngine;

namespace AshesAndSakura.Utils
{
    /// <summary>
    /// Simple camera controller for the battlefield
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float panSpeed = 20f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 10f;

        [Header("Boundaries")]
        [SerializeField] private float minX = -10f;
        [SerializeField] private float maxX = 10f;
        [SerializeField] private float minY = -5f;
        [SerializeField] private float maxY = 5f;

        private Camera cam;
        private Vector3 dragOrigin;
        private bool isDragging = false;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                Debug.LogError("[CameraController] No camera component found!");
            }
        }

        private void Update()
        {
            HandlePanning();
            HandleZoom();
        }

        private void HandlePanning()
        {
            // Mouse drag to pan (right click or middle mouse)
            if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
            {
                dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
                isDragging = true;
            }

            if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
            {
                if (isDragging)
                {
                    Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 newPosition = transform.position + difference;

                    // Clamp to boundaries
                    newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                    newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

                    transform.position = newPosition;
                }
            }

            if (Input.GetMouseButtonUp(2) || Input.GetMouseButtonUp(1))
            {
                isDragging = false;
            }

            // Keyboard panning (WASD or Arrow Keys)
            Vector3 keyboardPan = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                keyboardPan.y += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                keyboardPan.y -= 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                keyboardPan.x -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                keyboardPan.x += 1f;

            if (keyboardPan != Vector3.zero)
            {
                Vector3 newPosition = transform.position + keyboardPan * panSpeed * Time.deltaTime;
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
                transform.position = newPosition;
            }
        }

        private void HandleZoom()
        {
            if (cam == null) return;

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                float newSize = cam.orthographicSize - scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
        }
    }
}
