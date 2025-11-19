using System;
using UnityEngine;
using UnityEngine.UI; 

public class EnemyHealth : MonoBehaviour
{
    public event Action OnDeath; 
    public event Action<Vector3, float> OnKnockbackApplied; 
    
    [Header("Health Settings")]
    public int maxHP = 5;
    private int currentHP;
    
    [Header("UI Reference")]
    [SerializeField] private HealthBarUI healthBarUI; 

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitClip; 
    private bool isDead = false;

    public bool IsDead => isDead; 

    private void Awake()
    {
        if (healthBarUI == null)
        {
            healthBarUI = GetComponentInChildren<HealthBarUI>(true);
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        currentHP = maxHP;
        
        if (healthBarUI != null)
        {
            healthBarUI.SetMaxHealth(maxHP);
            healthBarUI.UpdateHealth(currentHP);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP = Mathf.Max(0, currentHP - damage);
        
        if (healthBarUI != null)
        {
            if (maxHP > 0) 
            {
                healthBarUI.UpdateHealth(currentHP);
            }
        }
        
        if (audioSource != null && hitClip != null)
        {
            audioSource.PlayOneShot(hitClip);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void RequestKnockback(Vector3 direction, float force)
    {
        if (isDead) return;
        OnKnockbackApplied?.Invoke(direction, force);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (healthBarUI != null)
        {
            healthBarUI.gameObject.SetActive(false); 
        }

        OnDeath?.Invoke();
    }
}