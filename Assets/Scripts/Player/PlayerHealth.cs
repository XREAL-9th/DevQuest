using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDeath;

    [Header("Optional Effects")]
    public AudioSource audioSource; 
    public AudioClip hitClip;
    public GameObject deathEffect;

    private bool isDead = false;
    private PlayerController controller;
    private PlayerShooting shooter;
    private Rigidbody rb;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        controller = GetComponent<PlayerController>();
        shooter = GetComponent<PlayerShooting>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (hitClip != null)
            audioSource.PlayOneShot(hitClip);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[Player] Dead");

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (controller != null) controller.enabled = false;
        if (shooter != null) shooter.enabled = false;

        if (rb != null) rb.linearVelocity = Vector3.zero;

        GameManager.Instance?.SetGameOver(false);

        OnPlayerDeath?.Invoke();
    }
}
