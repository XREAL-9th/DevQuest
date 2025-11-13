using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Gameplay UI")]
    [SerializeField] private TMP_Text timeText;          // 남은 시간 표시
    [SerializeField] private TMP_Text enemyCountText;    // 적 처치 / 남은 수 표시

    [Header("Game Settings")]
    [SerializeField] private int totalEnemies = 5;
    [SerializeField] private float gameTime = 120f; // 전체 제한 시간(초)

    private int defeatedEnemies = 0;
    private bool gameEnded = false;
    private float timer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        timer = gameTime;

        if (winPanel) winPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        totalEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
        Debug.Log($"Total enemies in scene: {totalEnemies}");

        UpdateUI();
    }


    private void Update()
    {
        if (gameEnded) return;

        // 남은 시간 갱신
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            GameOver(); // 시간이 다 되면 패배
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        // 시간 표시 (mm:ss 형식)
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timeText.text = $"Time Left: {minutes:00}:{seconds:00}";
        }

        // 적 처치 수 표시
        if (enemyCountText != null)
        {
            int remaining = Mathf.Max(0, totalEnemies - defeatedEnemies);
            enemyCountText.text = $"Enemies: {defeatedEnemies} / {totalEnemies}";
        }
    }

    // ------------------------------------------------------------
    // 적 사망 시 호출
    // ------------------------------------------------------------
    public void OnEnemyDefeated()
    {
        if (gameEnded) return;

        defeatedEnemies++;

        if (defeatedEnemies == totalEnemies)
        {
            WinGame();
        }
    }

    // ------------------------------------------------------------
    // 승리 / 패배 처리
    // ------------------------------------------------------------
    public void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (winPanel) winPanel.SetActive(true);
        Debug.Log("🎉 게임 승리!");
    }

    public void GameOver()
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (gameOverPanel) gameOverPanel.SetActive(true);
        Debug.Log("💀 게임 오버!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
