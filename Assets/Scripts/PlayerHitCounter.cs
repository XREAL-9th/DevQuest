using UnityEngine;

public class PlayerHitCounter : MonoBehaviour
{
    [SerializeField] int hp = 5;
    [SerializeField] GameObject gameOverUI;   // 게임오버 패널

    void Start()
    {
        if (gameOverUI)
            gameOverUI.SetActive(false); // 처음엔 꺼둠
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            hp--;
            Destroy(other.gameObject);

            if (hp <= 0)
            {
                GameManager.Instance.GameOver();
                GameOver();
            }
        }
    }

    void GameOver()
    {
        Time.timeScale = 0f; // 게임 정지(연출)
        if (gameOverUI)
            gameOverUI.SetActive(true);

        Debug.Log("GAME OVER");
    }
}
