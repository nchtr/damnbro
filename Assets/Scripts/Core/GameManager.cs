using UnityEngine;
using UnityEngine.SceneManagement;
using Damnbro.Player;

namespace Damnbro.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public HealthSystem playerHealth;
        public float respawnDelay = 2f;
        public bool reloadSceneOnDeath = true;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            if (playerHealth != null) playerHealth.OnDied += OnPlayerDied;
        }

        void OnDestroy()
        {
            if (playerHealth != null) playerHealth.OnDied -= OnPlayerDied;
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                    ? CursorLockMode.None
                    : CursorLockMode.Locked;
            }
        }

        void OnPlayerDied(GameObject source)
        {
            if (!reloadSceneOnDeath) return;
            Invoke(nameof(Reload), respawnDelay);
        }

        void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
