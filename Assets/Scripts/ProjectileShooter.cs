using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Header("발사체 설정")]
    public GameObject projectilePrefab;   // Rigidbody 포함된 프리팹
    public float projectileSpeed = 40f;   // 속도
    public float lifeTime = 3f;           // 생존 시간

    [Header("발사 위치 설정")]
    public Transform firePoint;           // 발사 위치 (카메라 앞 등)

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * projectileSpeed;
        }

        Destroy(projectile, lifeTime);
    }
}
