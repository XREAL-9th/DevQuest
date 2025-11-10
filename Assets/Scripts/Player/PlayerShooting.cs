using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("— Shooting Settings —")]
    public float range = 100f;            // 사거리
    public float damage = 1f;             // 1회 데미지
    public Camera fpsCam;                 // 카메라 참조

    [Header("— Visual Effects —")]
    public GameObject hitEffectPrefab;    // 피격 이펙트 (선택)

    void Update()
    {
        // 마우스 왼쪽 클릭 시 발사
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (fpsCam == null)
        {
            Debug.LogWarning("FPS Camera not assigned!");
            return;
        }

        // 화면 중앙에서 레이 발사
        Ray ray = fpsCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast로 충돌 감지
        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log($"Hit object: {hit.transform.name}");

            // ✅ 피격 이펙트 생성 (모든 표면에 적용 가능)
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // ✅ Enemy 피격 판정
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit detected!");

                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // 이미 사망한 적은 무시
                    var enemyDeadField = enemy.GetType().GetField("isDead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    bool isDead = enemyDeadField != null && (bool)enemyDeadField.GetValue(enemy);
                    if (!isDead)
                    {
                        enemy.TakeDamage((int)damage);
                        Debug.Log($"Enemy took {damage} damage!");
                    }
                    else
                    {
                        Debug.Log("Hit ignored — enemy already dead.");
                    }
                }

                // ✅ 넉백 효과 (선택)
                Rigidbody enemyRb = hit.transform.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    Vector3 knockDir = (hit.transform.position - ray.origin).normalized;
                    enemyRb.AddForce(knockDir * 5f, ForceMode.Impulse);
                }
            }
        }
        else
        {
            Debug.Log("No hit detected.");
        }
    }
}
