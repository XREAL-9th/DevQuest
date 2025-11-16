using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player HP UI")]
    [SerializeField] private TMP_Text playerHealthText;

    [Header("Game UI")]
    [SerializeField] private TMP_Text timeLeftText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text fpsText;

    [Header("GameEnd UI")]
    [SerializeField] private GameObject winScreenPanel;
    [SerializeField] private GameObject loseScreenPanel;
    [SerializeField] private TMP_Text winScreenTimeText;

    private float fpsTimer;

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
        winScreenPanel.SetActive(false);
        loseScreenPanel.SetActive(false);
    }

    void Update()
    {
        fpsTimer -= Time.unscaledDeltaTime;
        if (fpsTimer <= 0)
        {
            float ms = Time.unscaledDeltaTime * 1000f;
            float fps = 1f / Time.unscaledDeltaTime;
            fpsText.text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);
            fpsTimer = 0.5f;
        }
    }

    public void UpdatePlayerHP(float currentHealth, float maxHealth)
    {
        playerHealthText.text = $"{Mathf.CeilToInt(currentHealth)} / {maxHealth}";
    }

    public void UpdateTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timeLeftText.text = $"{minutes:00}:{seconds:00}";
    }

    public void UpdateScore(int currentKills, int goalKills)
    {
        scoreText.text = $"Kills: {currentKills} / {goalKills}";
    }

    public void ShowWinScreen(float remainingTime)
    {
        winScreenPanel.SetActive(true);
        loseScreenPanel.SetActive(false);

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        winScreenTimeText.text = $"Remain Time: {minutes:00}:{seconds:00}";
    }

    public void ShowLoseScreen()
    {
        winScreenPanel.SetActive(false);
        loseScreenPanel.SetActive(true);
    }

    public void OnRestartButtonPushed()
    {
        GameManager.Instance.RestartGame();
    }
}
