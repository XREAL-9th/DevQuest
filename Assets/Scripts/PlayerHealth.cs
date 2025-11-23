using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI - Inspector에서 연결")]
    public Image healthBarFill;
    public Text healthText;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;

            if (currentHealth / maxHealth > 0.5f)
                healthBarFill.color = Color.green;
            else if (currentHealth / maxHealth > 0.2f)
                healthBarFill.color = Color.yellow;
            else
                healthBarFill.color = Color.red;
        }

        if (healthText != null)
        {
            healthText.text = $"HP: {(int)currentHealth}";
        }
    }

    void Die()
    {
        Debug.Log("플레이어 사망!");
    }
}