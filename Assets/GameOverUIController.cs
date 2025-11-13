using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Texts")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);

        // GameManager 이벤트 등록
        GameManager.Instance.OnWin += ShowWin;
        GameManager.Instance.OnLose += ShowLose;
    }

    void ShowWin()
    {
        if (winPanel) winPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    void ShowLose()
    {
        if (losePanel) losePanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    // 🔁 버튼에서 연결할 함수
    public void Restart()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
