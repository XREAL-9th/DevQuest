using System;
using UnityEditor.UIElements;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private bool isDead = false;
    public event Action OnDied;
    public event Action<float> OnDamaged;
    public event Action<float, float> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log(currentHealth.ToString());
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        OnDamaged?.Invoke(damage);
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDied?.Invoke();
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
