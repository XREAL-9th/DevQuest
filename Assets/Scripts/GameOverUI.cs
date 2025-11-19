using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel; // 전체 패널
    [SerializeField] private TextMeshProUGUI titleText; // 승리/패배 제목
    [SerializeField] private Button restartButton; // 재시작 버튼

    private void Awake()
    {
        panel.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += ShowGameOverScreen;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= ShowGameOverScreen;
        }
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }
    }

    private void ShowGameOverScreen(bool win)
    {
        panel.SetActive(true);

        if (titleText != null)
        {
            if (win)
            {
                titleText.text = "VICTORY!";
                titleText.color = Color.green;
            }
            else
            {
                titleText.text = "DEFEAT";
                titleText.color = Color.red;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}
