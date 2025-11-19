using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 1;

    [HideInInspector] public GameObject owner;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = false;

            rb.mass = 1f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Fire(Vector3 dir)
    {
        if (rb)
        {
            rb.linearVelocity = dir * speed;
        }
        else
        {
            Debug.LogError("[EnemyProjectile] Rigidbody missing!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1) 자신(owner) 무시
        if (owner != null && collision.gameObject == owner)
            return;

        // 2) 다른 EnemyProjectile 무시
        if (collision.gameObject.GetComponent<EnemyProjectile>() != null)
            return;

        // 3) 플레이어 충돌 판정
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("🎯 EnemyProjectile hit Player!");

            PlayerHealth ph = null;

            // 🔥 (우선) PlayerHealth.Instance가 살아있으면 이것을 사용
            if (PlayerHealth.Instance != null)
            {
                ph = PlayerHealth.Instance;
            }
            else
            {
                // 🔥 fallback: 충돌한 오브젝트 또는 부모에서 PlayerHealth 찾기
                ph = collision.gameObject.GetComponent<PlayerHealth>();
                if (ph == null)
                {
                    ph = collision.gameObject.GetComponentInParent<PlayerHealth>();
                }
            }

            if (ph != null)
            {
                ph.TakeDamage(damage);
                Debug.Log($"💥 Player HP -{damage} → {ph.currentHealth}");
            }
            else
            {
                Debug.LogError("🚨 PlayerHealth component not found on Player!");
            }
        }

        Destroy(gameObject);
    }
}
