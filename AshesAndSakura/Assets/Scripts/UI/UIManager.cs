using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AshesAndSakura.Managers;

namespace AshesAndSakura.UI
{
    /// <summary>
    /// Main UI Manager for the game - handles all UI updates and interactions
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Top Bar UI")]
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI cpText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Unit Slots UI")]
        [SerializeField] private Transform unitSlotsContainer;
        [SerializeField] private GameObject unitSlotPrefab;

        [Header("Skills Panel UI")]
        [SerializeField] private Transform skillsContainer;
        [SerializeField] private GameObject skillSlotPrefab;

        [Header("Debug Panel")]
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private bool showDebug = true;

        private float gameTime = 0f;

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
        }

        private void Start()
        {
            InitializeUI();
            SubscribeToEvents();
            PopulateUnitSlots();
            PopulateSkillSlots();
        }

        private void Update()
        {
            UpdateTimer();
            UpdateDebugInfo();
        }

        private void InitializeUI()
        {
            UpdateWaveDisplay(1);
            UpdateCPDisplay(100);
            Debug.Log("[UIManager] UI Initialized");
        }

        private void SubscribeToEvents()
        {
            // Subscribe to ResourceManager events
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnCPChanged += UpdateCPDisplay;
            }

            // Subscribe to WaveManager events
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnWaveStarted += UpdateWaveDisplay;
                WaveManager.Instance.OnRandomEvent += ShowRandomEvent;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnCPChanged -= UpdateCPDisplay;
            }

            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnWaveStarted -= UpdateWaveDisplay;
                WaveManager.Instance.OnRandomEvent -= ShowRandomEvent;
            }
        }

        private void UpdateTimer()
        {
            gameTime += Time.deltaTime;
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(gameTime / 60f);
                int seconds = Mathf.FloorToInt(gameTime % 60f);
                timerText.text = $"{minutes:00}:{seconds:00}";
            }
        }

        private void UpdateWaveDisplay(int wave)
        {
            if (waveText != null)
            {
                waveText.text = $"WAVE {wave}";
            }
            Debug.Log($"[UIManager] Wave display updated: {wave}");
        }

        private void UpdateCPDisplay(int currentCP)
        {
            if (cpText != null)
            {
                int maxCP = ResourceManager.Instance != null ? ResourceManager.Instance.MaxCP : 999;
                cpText.text = $"CP: {currentCP}/{maxCP}";
            }
        }

        private void PopulateUnitSlots()
        {
            if (unitSlotsContainer == null || UnitManager.Instance == null)
            {
                Debug.LogWarning("[UIManager] Cannot populate unit slots - missing references");
                return;
            }

            var units = UnitManager.Instance.GetAllUnits();
            Debug.Log($"[UIManager] Populating {units.Length} unit slots");

            foreach (var unitData in units)
            {
                GameObject slot = Instantiate(unitSlotPrefab, unitSlotsContainer);

                // Set up unit slot with data
                var dragHandler = slot.GetComponent<DragDropHandler>();
                if (dragHandler != null)
                {
                    dragHandler.SetUnitData(unitData.unitId, unitData);
                }

                // Update visual (if using TextMeshPro for labels)
                var label = slot.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = $"{unitData.unitName}\n{unitData.cpCost} CP";
                }

                // Update icon color
                var icon = slot.GetComponentInChildren<Image>();
                if (icon != null)
                {
                    icon.color = unitData.unitColor;
                }
            }
        }

        private void PopulateSkillSlots()
        {
            if (skillsContainer == null || SkillManager.Instance == null)
            {
                Debug.LogWarning("[UIManager] Cannot populate skill slots - missing references");
                return;
            }

            var skills = SkillManager.Instance.GetAllSkills();
            Debug.Log($"[UIManager] Populating {skills.Count} skill slots");

            foreach (var skill in skills)
            {
                GameObject slot = Instantiate(skillSlotPrefab, skillsContainer);

                var button = slot.GetComponent<Button>();
                if (button != null)
                {
                    string skillId = skill.skillId;
                    button.onClick.AddListener(() => OnSkillButtonClicked(skillId));
                }

                var label = slot.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                {
                    label.text = $"{skill.skillName}\n{skill.cpCost} CP";
                }
            }
        }

        private void OnSkillButtonClicked(string skillId)
        {
            // For now, activate at center of screen
            Vector3 center = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            center.z = 0f;

            if (SkillManager.Instance != null)
            {
                SkillManager.Instance.TryActivateSkill(skillId, center);
            }
        }

        private void ShowRandomEvent(string eventName)
        {
            Debug.Log($"[UIManager] Random Event: {eventName}");
            // TODO: Show event notification UI
        }

        private void UpdateDebugInfo()
        {
            if (!showDebug || debugText == null) return;

            string info = "=== DEBUG INFO ===\n";

            if (ResourceManager.Instance != null)
            {
                info += $"CP: {ResourceManager.Instance.CurrentCP}/{ResourceManager.Instance.MaxCP}\n";
            }

            if (WaveManager.Instance != null)
            {
                info += $"Wave: {WaveManager.Instance.CurrentWave}\n";
                info += $"Enemies Remaining: {WaveManager.Instance.EnemiesRemaining}\n";
            }

            if (UnitManager.Instance != null)
            {
                info += $"Active Units: {UnitManager.Instance.ActiveUnits.Count}\n";
            }

            if (EnemySpawner.Instance != null)
            {
                info += $"Active Enemies: {EnemySpawner.Instance.ActiveEnemies.Count}\n";
            }

            debugText.text = info;
        }
    }
}
