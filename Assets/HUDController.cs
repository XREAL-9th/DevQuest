using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Refs")]
    public PlayerHitCounter player;       // 플레이어
    public Slider hpBar;                  // HP바
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI timeText;

    [Header("Timer")]
    public float timeLimit = 60f;         // 제한 시간 (원하면 조정)
    float timeLeft;

    void Start()
    {
        timeLeft = timeLimit;
    }

    void Update()
    {
        // 🕐 시간 갱신
        if (!GameManager.Instance.isGameOver)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = 0;
                GameManager.Instance.Lose(); // 시간 초과 시 패배
            }
        }

        // 🩸 HP 갱신
        if (player && hpBar)
            hpBar.value = Mathf.Clamp01((float)player.hp / 5f); // 현재 HP/최대HP 비율

        // 🎯 점수 갱신
        if (scoreText)
            scoreText.text = $"Score: {GameManager.Instance.score}";

        // 💀 킬 / 목표 킬
        if (killsText)
            killsText.text = $"Kills: {GameManager.Instance.killCount}/{GameManager.Instance.targetKillCount}";

        // 👾 남은 적 / 전체 적
        if (enemiesText)
            enemiesText.text = $"Enemies: {GameManager.Instance.aliveEnemies}/{GameManager.Instance.totalEnemies}";

        // ⏱ 남은 시간
        if (timeText)
            timeText.text = $"Time: {timeLeft:0.0}s";
    }
}
