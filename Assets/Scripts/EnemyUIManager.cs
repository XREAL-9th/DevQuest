using UnityEngine;
using UnityEngine.UI;

public class EnemyUIManager : MonoBehaviour
{
    [Header("HP Settings")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject damageTextPrefab;

    private Health targetHealth;

    private void Awake()
    {
        targetHealth = GetComponent<Health>();
        if (targetHealth == null || hpSlider == null)
        {
            Debug.Log(" hp ui is null");
            enabled = false;
            return;
        }
        hpSlider.maxValue = targetHealth.maxHealth;

        targetHealth.OnHealthChanged += UpdateHealthBar;
        targetHealth.OnDamaged += ShowDamageText;
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        hpSlider.value = currentHealth;
        hpSlider.maxValue = targetHealth.maxHealth;
    }

    private void ShowDamageText(float damage)
    {
        if (damageTextPrefab == null) return;

        Transform targetCanvas = hpSlider.GetComponentInParent<Canvas>()?.transform;
        GameObject textGO = Instantiate(damageTextPrefab, targetCanvas);
        DamageText animator = textGO.GetComponent<DamageText>();

        if (animator != null)
        {
            animator.SetDamageValue(damage);
        }
    }

    private void OnDestroy()
    {
        if(targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateHealthBar;
            targetHealth.OnDamaged -= ShowDamageText;
        }
    }
}
