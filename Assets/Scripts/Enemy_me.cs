using UnityEngine;

public class Enemy_me: MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float moveSpeed = 2f;
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;

    [Header("UI - Inspector에서 연결")]
    public UnityEngine.UI.Image healthBarFill;
    public Transform healthBarCanvas;

    private Transform player;
    private float lastAttackTime = 0f;
    private Rigidbody rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();

        // 플레이어 찾기 (XR Origin)
        GameObject xrOrigin = GameObject.Find("XR Origin (VR)");
        if (xrOrigin == null) xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin != null)
        {
            player = xrOrigin.transform;
        }

        UpdateHealthUI();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 체력바 항상 플레이어 쪽 보기
        if (healthBarCanvas != null)
        {
            healthBarCanvas.LookAt(player);
        }

        if (distanceToPlayer > attackRange)
        {
            // 플레이어에게 이동
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            rb.velocity = new Vector3(direction.x * moveSpeed, rb.velocity.y, direction.z * moveSpeed);
        }
        else
        {
            // 공격 범위 안에 있으면 공격
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        Debug.Log("적이 플레이어를 공격!");

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
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
        }
    }

    void Die()
    {
        Debug.Log("적 사망!");
        Destroy(gameObject);
    }
}
