using UnityEngine;
using UnityEngine.UI; // HP바 등 UI를 쓸 경우

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("UI (Optional)")]
    public Slider healthBar; 

    [Header("Effects")]
    public GameObject deathEffect;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player Died!");

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        gameObject.SetActive(false);

        // 게임오버 화면 호출
        GameManager.Instance.GameOver();
    }
}
