using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Player HP UI")]
    public TMP_Text healthText;

    [Header("Game UI")]
    public TMP_Text timeText;
    public TMP_Text killCountText;
    public GameObject gameOverPanel;

    private void Start()
    {
        if (PlayerHealth.Instance != null)
        {
            healthText.text =
                $"HP: {PlayerHealth.Instance.currentHealth} / {PlayerHealth.Instance.maxHealth}";
        }

        GameManager.Instance.OnTimeUpdated += UpdateTimeUI;
        GameManager.Instance.OnEnemyDefeated += UpdateKillCountUI;
        GameManager.Instance.OnGameOver += ShowGameOverUI;

        UpdateKillCountUI(GameManager.Instance.enemiesDefeated, GameManager.Instance.TargetEnemyCount);

        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (PlayerHealth.Instance != null)
        {
            healthText.text =
                $"HP: {PlayerHealth.Instance.currentHealth} / {PlayerHealth.Instance.maxHealth}";
        }
    }

    private void UpdateTimeUI(float timeLeft)
    {
        timeText.text = $"Time: {timeLeft:F1}s";
    }

    private void UpdateKillCountUI(int defeated, int target)
    {
        killCountText.text = $"Kills: {defeated} / {target}";
    }

    private void ShowGameOverUI(bool win)
    {
        gameOverPanel.SetActive(true);
    }
}
