using UnityEngine;
using UnityEngine.UI;

namespace AshesAndSakura.Utils
{
    /// <summary>
    /// Simple health bar for units and enemies
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("Health Bar Settings")]
        [SerializeField] private Image fillImage;
        [SerializeField] private Color fullHealthColor = Color.green;
        [SerializeField] private Color lowHealthColor = Color.red;
        [SerializeField] private float lowHealthThreshold = 0.3f;

        [Header("Display Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
        [SerializeField] private bool hideWhenFull = true;
        [SerializeField] private bool faceCamera = true;

        private Transform targetTransform;
        private Canvas canvas;
        private float currentHealthRatio = 1f;

        private void Awake()
        {
            canvas = GetComponentInChildren<Canvas>();
            if (canvas != null && faceCamera)
            {
                canvas.worldCamera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            if (targetTransform != null)
            {
                transform.position = targetTransform.position + offset;
            }

            if (faceCamera && Camera.main != null)
            {
                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
            }

            // Hide when at full health
            if (hideWhenFull && canvas != null)
            {
                canvas.enabled = currentHealthRatio < 1f;
            }
        }

        /// <summary>
        /// Set the target transform to follow
        /// </summary>
        public void SetTarget(Transform target)
        {
            targetTransform = target;
        }

        /// <summary>
        /// Update the health bar display
        /// </summary>
        public void UpdateHealth(int current, int max)
        {
            currentHealthRatio = (float)current / max;
            currentHealthRatio = Mathf.Clamp01(currentHealthRatio);

            if (fillImage != null)
            {
                fillImage.fillAmount = currentHealthRatio;

                // Change color based on health
                if (currentHealthRatio <= lowHealthThreshold)
                {
                    fillImage.color = lowHealthColor;
                }
                else
                {
                    fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor,
                        (currentHealthRatio - lowHealthThreshold) / (1f - lowHealthThreshold));
                }
            }
        }
    }
}
