using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("ScriptableObject 데이터 참조")]
    public BulletData bulletData;

    private Rigidbody rb;
    private float lifeTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (bulletData == null)
        {
            Debug.LogWarning($"{name}: BulletData was null, disabling bullet.");
            gameObject.SetActive(false);
            return;
        }

        lifeTimer = 0f;
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (bulletData == null)
            return;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= bulletData.lifetime)
        {
            if (BulletPool.Instance != null)
                BulletPool.Instance.ReturnToPool(gameObject);
            else
                Destroy(gameObject);
        }
    }

    public void Fire(Vector3 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (bulletData == null)
        {
            Debug.LogError($"{name}: BulletData missing, cannot fire!");
            return;
        }

        rb.linearVelocity = direction * bulletData.speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                Vector3 knockDir = (collision.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockDir * bulletData.knockbackForce, ForceMode.Impulse);
            }

            if (bulletData.hitEffectPrefab != null)
            {
                Instantiate(
                    bulletData.hitEffectPrefab,
                    collision.contacts[0].point,
                    Quaternion.LookRotation(collision.contacts[0].normal)
                );
            }
        }

        if (BulletPool.Instance != null)
            BulletPool.Instance.ReturnToPool(gameObject);
        else
            Destroy(gameObject);
    }
}
