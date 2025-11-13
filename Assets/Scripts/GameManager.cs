using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject enemyCountUI; // 적 카운트 UI 오브젝트

    [Header("Game State")]
    private int totalEnemies = 0;
    private int deadEnemies = 0;
    private bool gameEnded = false;

    private void Awake()
    {
        // 싱글톤 패턴
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
        // 초기화
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // 씬에 있는 모든 적 카운트
        CountEnemies();

        // 적 카운트 UI 업데이트
        UpdateEnemyCountUI();
    }

    private void CountEnemies()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        totalEnemies = enemies.Length;
        Debug.Log($"[GameManager] 총 적의 수: {totalEnemies}");
    }

    private void UpdateEnemyCountUI()
    {
        if (enemyCountUI != null)
        {
            int remainingEnemies = totalEnemies - deadEnemies;

            // GameObject 안에 있는 Text 또는 TextMeshPro 컴포넌트를 찾아서 업데이트
            Text textComponent = enemyCountUI.GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = $"적: {remainingEnemies} / {totalEnemies}";
            }

            // TextMeshPro 사용하는 경우
            var tmpComponent = enemyCountUI.GetComponent<TMPro.TextMeshProUGUI>();
            if (tmpComponent != null)
            {
                tmpComponent.text = $"적: {remainingEnemies} / {totalEnemies}";
            }
        }
    }

    public void RegisterEnemyDeath()
    {
        if (gameEnded) return;

        deadEnemies++;
        Debug.Log($"[GameManager] 적 사망! ({deadEnemies}/{totalEnemies})");

        // 적 카운트 UI 업데이트
        UpdateEnemyCountUI();

        if (deadEnemies >= totalEnemies)
        {
            Victory();
        }
    }

    public void GameOver()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("[GameManager] 게임 오버!");

        // 적 카운트 UI 숨기기
        if (enemyCountUI != null)
        {
            enemyCountUI.SetActive(false);
        }

        // 게임오버 패널 표시
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 게임 일시정지
        Time.timeScale = 0f;

        // 3초 후 재시작 (선택사항)
        // Invoke("RestartGame", 3f);
    }

    private void Victory()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("[GameManager] 승리!");

        // 적 카운트 UI 숨기기
        if (enemyCountUI != null)
        {
            enemyCountUI.SetActive(false);
        }

        // 승리 패널 표시
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // 게임 일시정지
        Time.timeScale = 0f;

        // 3초 후 다음 레벨 or 재시작 (선택사항)
        // Invoke("NextLevel", 3f);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        // 다음 씬으로 이동
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // 마지막 레벨이면 첫 레벨로
            SceneManager.LoadScene(0);
        }
    }

    /*public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }*/
}