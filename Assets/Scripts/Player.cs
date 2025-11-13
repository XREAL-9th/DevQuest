using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Health UI")]
    [SerializeField] private Image healthBarFill; // Screen Space UI
    [SerializeField] private Text healthText; // 선택사항

    [Header("Death")]
    [SerializeField] private GameObject deathEffect;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"[Player] TakeDamage 호출됨! damage: {damage}, isDead: {isDead}");

        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"플레이어 피해: {damage}, 남은 체력: {currentHealth}/{maxHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;

            // 체력에 따라 색상 변경
            if (currentHealth / maxHealth > 0.5f)
                healthBarFill.color = Color.green;
            else if (currentHealth / maxHealth > 0.2f)
                healthBarFill.color = Color.yellow;
            else
                healthBarFill.color = Color.red;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth:F0} / {maxHealth:F0}";
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("플레이어 사망!");

        // 사망 이펙트
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // GameManager에 게임 오버 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            // GameManager가 없을 경우 기본 동작
            Time.timeScale = 0;
        }

        // gameObject.SetActive(false); // 플레이어 비활성화
    }
}