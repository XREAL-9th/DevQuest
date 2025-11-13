using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int totalEnemies;
    private int defeatedEnemies;
    private bool gameEnded;

    [Header("UI References")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winText;

    [SerializeField] private TextMeshProUGUI enemyCountText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        defeatedEnemies = 0;
        gameEnded = false;

        if (winPanel != null)
            winPanel.SetActive(false);

        UpdateEnemyCountUI();
    }

    public void OnEnemyDied()
    {
        if (gameEnded) return;

        defeatedEnemies++;
        Debug.Log($"Enemy defeated {defeatedEnemies}/{totalEnemies}");

        UpdateEnemyCountUI();

        if (defeatedEnemies >= totalEnemies)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameEnded = true;
        Debug.Log("GAME CLEAR");

        if (winPanel != null)
            winPanel.SetActive(true);

        if (winText != null)
            winText.text = "YOU WIN!";

        Time.timeScale = 0f;
    }

    void UpdateEnemyCountUI()
    {
        if (enemyCountText == null) return;

        int remaining = Mathf.Max(0, totalEnemies - defeatedEnemies);
        enemyCountText.text = $"Defeated: {defeatedEnemies}   Remaining: {remaining}/{totalEnemies}";
    }
}