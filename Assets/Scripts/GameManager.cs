using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;   // 싱글턴 접근용
    private int totalEnemies;              // 전체 적 수
    private int defeatedEnemies;           // 죽은 적 수

    void Awake()
    {
        // 싱글턴 초기화
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Enemy 태그 가진 오브젝트를 모두 세기
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        defeatedEnemies = 0;
    }

    public void OnEnemyDied()
    {
        defeatedEnemies++;
        Debug.Log($"Enemy defeated {defeatedEnemies}/{totalEnemies}");

        if (defeatedEnemies >= totalEnemies)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER - 모든 적 처치!");
    }
}