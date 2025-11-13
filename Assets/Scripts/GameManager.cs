using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float limitTime = 60f;   // 제한 시간
    public int totalEnemies;        // 스폰된 적 수
    private int killedEnemies = 0;

    private float timer;
    private bool gameEnded = false;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI enemyCountText;
    public GameObject winUI;
    public GameObject loseUI;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        timer = limitTime;
    }

    private void Start()
    {
        if (winUI) winUI.SetActive(false);
        if (loseUI) loseUI.SetActive(false);
        totalEnemies = 10;
        UpdateEnemyUI();
    }

    private void Update()
    {
        if (gameEnded) return;

        // Timer
        timer -= Time.unscaledDeltaTime;
        if (timer < 0)
        {
            Lose();
            return;
        }

        if (timerText)
            timerText.text = $"Time: {timer:F1}";
    }

    public void EnemyKilled()
    {
        killedEnemies++;
        UpdateEnemyUI();

        if (killedEnemies >= totalEnemies)
            Win();
    }

    private void UpdateEnemyUI()
    {
        if (enemyCountText)
            enemyCountText.text = $"Enemy: {killedEnemies}/{totalEnemies}";
    }

    public void Win()
    {
        gameEnded = true;
        Time.timeScale = 0f;
        if (winUI) winUI.SetActive(true);
    }

    public void Lose()
    {
        gameEnded = true;
        Time.timeScale = 0f;
        if (loseUI) loseUI.SetActive(true);
    }
}
