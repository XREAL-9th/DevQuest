using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스를 위한 정적 변수 선언
    public static GameManager Instance { get; private set; }
    public int score;  // 점수(죽인 적 수)
    public GameObject YouWinPanel;

    public TextMeshProUGUI currentScoreText;

    void Awake()
    {
        // 이미 인스턴스가 존재하면 현재 인스턴스 파괴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // 인스턴스가 존재하지 않으면 현재 인스턴스를 할당
        Instance = this;
        // 씬 전환 시에도 게임 오브젝트가 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore()
    {
        score++;
        currentScoreText.text = score.ToString(); // 점수 텍스트 업데이트
        Debug.Log("현재 점수: " + score);
        if(score >= 10)
        {
            Debug.Log("You Win!");
            YouWinPanel.SetActive(true);
            Time.timeScale = 0;  // 게임 일시 정지
        }
    }
}
