using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private bool isDead = false;
    public event Action OnDied;
    public event Action OnDamaged;
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

        if (currentHealth <= 0) Die();
        else
        {
            OnDamaged?.Invoke();
        }
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
