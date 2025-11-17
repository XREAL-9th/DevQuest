using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GameState
    {
        None,
        Playing,
        Won,
        Lost
    }

    [Header("Game Setting")]
    [SerializeField] private float gameTimeLimit = 120f;
    [SerializeField] private int goalKills = 10;

    public GameState CurrentGameState { get; private set; }
    private float timeLeft;
    private int currentKills;
    private Health playerHealth;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        CurrentGameState = GameState.Playing;
        timeLeft = gameTimeLimit;
        currentKills = 0;
        Time.timeScale = 1f;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnDied += OnPlayerDied;
                playerHealth.OnHealthChanged += UIManager.Instance.UpdatePlayerHP;
                UIManager.Instance.UpdatePlayerHP(playerHealth.GetCurrentHealth(), playerHealth.maxHealth);
            }
        }

        UIManager.Instance.UpdateScore(currentKills, goalKills);
        UIManager.Instance.UpdateTime(timeLeft);
    }

    void Update()
    {
        if (CurrentGameState != GameState.Playing) return;

        timeLeft -= Time.deltaTime;
        if(timeLeft < 0)
        {
            timeLeft = 0;
            HandleGameOver();
        }

        UIManager.Instance.UpdateTime(timeLeft);
    }

    public void OnEnemyKilled()
    {
        if (CurrentGameState != GameState.Playing) return;

        currentKills++;
        UIManager.Instance.UpdateScore(currentKills, goalKills);

        if (currentKills >= goalKills)
        {
            HandleGameWin();
        }
    }

    private void OnPlayerDied()
    {
        HandleGameOver();
    }

    private void HandleGameWin()
    {
        CurrentGameState = GameState.Won;
        Time.timeScale = 0f; 
        UIManager.Instance.ShowWinScreen(timeLeft);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HandleGameOver()
    {
        CurrentGameState = GameState.Lost;
        Time.timeScale = 0f;
        UIManager.Instance.ShowLoseScreen();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGame()
    {
        Debug.Log("RESTART");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
