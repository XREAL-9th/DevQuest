using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 5;
    private int currentHP;

    [Header("UI References")]
    public Slider hpSlider;
    public Canvas hpCanvas;

    private Animator animator;

    private void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();

        // UI 초기값
        if (hpSlider)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        currentHP -= amount;

        // Update HP bar
        if (hpSlider)
            hpSlider.value = currentHP;

        // 스턴 애니메이션
        if (animator)
            animator.SetTrigger("stun");

        Debug.Log($"Enemy Hit! HP: {currentHP}");

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        GameManager.Instance.EnemyKilled(); // 처치된 적 수 +1
        Destroy(gameObject); // 적과 HP UI 모두 제거됨
    }
}