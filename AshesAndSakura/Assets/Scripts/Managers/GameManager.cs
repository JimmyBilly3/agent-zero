using UnityEngine;
using UnityEngine.SceneManagement;

namespace AshesAndSakura.Managers
{
    /// <summary>
    /// Main GameManager that controls game state and lifecycle
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private bool permadeathMode = false;
        [SerializeField] private int playerBaseHealth = 100;

        [Header("Game State")]
        private GameState currentState = GameState.MainMenu;
        private int currentBaseHealth;
        private bool isPaused = false;

        public event System.Action OnGameStart;
        public event System.Action OnGameOver;
        public event System.Action OnGameWin;
        public event System.Action<bool> OnPauseStateChanged;

        public GameState CurrentState => currentState;
        public bool IsPaused => isPaused;
        public int BaseHealth => currentBaseHealth;
        public bool PermadeathMode => permadeathMode;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            currentBaseHealth = playerBaseHealth;
        }

        private void Start()
        {
            Debug.Log("[GameManager] Game initialized");
            Debug.Log($"[GameManager] Permadeath Mode: {permadeathMode}");
        }

        private void Update()
        {
            // Handle pause input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }

            // Debug controls
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StartGame();
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                RestartGame();
            }
        }

        /// <summary>
        /// Start a new game
        /// </summary>
        public void StartGame()
        {
            currentState = GameState.Playing;
            currentBaseHealth = playerBaseHealth;
            isPaused = false;

            Debug.Log("[GameManager] Game Started!");
            OnGameStart?.Invoke();
        }

        /// <summary>
        /// Restart the current game
        /// </summary>
        public void RestartGame()
        {
            Debug.Log("[GameManager] Restarting game...");

            // Clear all units and enemies
            if (UnitManager.Instance != null)
                UnitManager.Instance.ClearAllUnits();

            if (EnemySpawner.Instance != null)
                EnemySpawner.Instance.ClearAllEnemies();

            // Reset resources
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.ResetCP();

            // Reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Trigger game over
        /// </summary>
        public void GameOver()
        {
            if (currentState == GameState.GameOver) return;

            currentState = GameState.GameOver;
            Debug.Log("[GameManager] GAME OVER!");
            OnGameOver?.Invoke();

            if (permadeathMode)
            {
                Debug.Log("[GameManager] Permadeath mode - resetting to main menu");
                // TODO: Return to main menu
            }
        }

        /// <summary>
        /// Trigger victory
        /// </summary>
        public void Victory()
        {
            if (currentState == GameState.Victory) return;

            currentState = GameState.Victory;
            Debug.Log("[GameManager] VICTORY!");
            OnGameWin?.Invoke();
        }

        /// <summary>
        /// Damage the player base
        /// </summary>
        public void DamageBase(int damage)
        {
            currentBaseHealth -= damage;
            Debug.LogWarning($"[GameManager] Base took {damage} damage! HP: {currentBaseHealth}/{playerBaseHealth}");

            if (currentBaseHealth <= 0)
            {
                GameOver();
            }
        }

        /// <summary>
        /// Toggle pause state
        /// </summary>
        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;

            Debug.Log($"[GameManager] Game {(isPaused ? "Paused" : "Resumed")}");
            OnPauseStateChanged?.Invoke(isPaused);
        }

        /// <summary>
        /// Set pause state directly
        /// </summary>
        public void SetPause(bool pause)
        {
            if (isPaused == pause) return;
            TogglePause();
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("[GameManager] Quitting game...");
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        private void OnApplicationQuit()
        {
            Debug.Log("[GameManager] Application closing");
        }
    }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
}
