using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("발사체 설정")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float explosionRadius = 0f; // 0이면 직격만

    [Header("이펙트")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private TrailRenderer trail;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 충돌 지점
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;

        // 범위 데미지 (explosionRadius > 0일 때)
        if (explosionRadius > 0)
        {
            Collider[] hitColliders = Physics.OverlapSphere(hitPoint, explosionRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
        else
        {
            // 직격 데미지만
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        // 피격 이펙트 생성
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(effect, 2f);
        }

        // 발사체 삭제
        Destroy(gameObject);
    }
}