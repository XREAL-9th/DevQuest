using UnityEngine;
using UnityEngine.UI;

public class PlayerHitCounter : MonoBehaviour
{
    [SerializeField] int maxHP = 5;
    [SerializeField] GameObject gameOverUI;   // 게임오버 패널
    [SerializeField] Slider hpBar;            // HP바 (HUD에 있는 슬라이더 연결)

    public int hp;

    void Start()
    {
        hp = maxHP;
        if (gameOverUI)
            gameOverUI.SetActive(false);

        if (hpBar)
        {
            hpBar.minValue = 0;
            hpBar.maxValue = maxHP;
            hpBar.value = hp;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            hp--;
            Destroy(other.gameObject);

            if (hpBar)
                hpBar.value = hp;   // ✅ UI 반영

            if (hp <= 0)
            {
                GameManager.Instance.GameOver();
                GameOver();
            }
        }
    }

    void GameOver()
    {
        Time.timeScale = 0f;
        if (gameOverUI)
            gameOverUI.SetActive(true);

        Debug.Log("GAME OVER");
    }

    public float HPPercent => (float)hp / maxHP;
}
