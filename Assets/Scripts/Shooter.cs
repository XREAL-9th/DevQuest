using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;                 // Main Camera
    public Transform firePoint;        // 총구(빈 오브젝트)
    public GameObject bulletPrefab;

    [Header("Settings")]
    public float bulletSpeed = 30f;
    public float maxDistance = 200f;
    public LayerMask aimMask;          // 맞을 레이어(Enemy, Ground 등) — Player/Gun은 제외!
    public float spawnOffset = 0.2f;   // 총구에서 약간 앞으로 빼기(자기충돌 방지)

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver)
            return;  // 게임오버면 총 발사 막기

        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    void Shoot()
    {
        // 1) 화면 중앙 고정 조준(원하면 ScreenPointToRay로 클릭 지점 사용)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // 2) Player/Gun 레이어 제외한 aimMask로만 Raycast
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, aimMask, QueryTriggerInteraction.Ignore))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(maxDistance);

        // 3) 방향 계산 (총구에서 목표점까지)
        Vector3 dir = (targetPoint - firePoint.position).normalized;

        // 4) 총알을 총구에서 약간 앞으로 빼서 생성(자기충돌 방지)
        Vector3 spawnPos = firePoint.position + dir * spawnOffset;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, rot);

        // 5) 혹시 부모로 붙을 일 없게 보장(일반적으로 필요 없지만 안전)
        bullet.transform.SetParent(null);

        // 6) 속도 초기화 후, 순간 속도 부여
        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;                         // 이전 값 리셋
            rb.angularVelocity = Vector3.zero;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.useGravity = false;                              // 원하는 경우 켜세요
            rb.AddForce(dir * bulletSpeed, ForceMode.VelocityChange);
        }

        // 디버그 확인(씬 뷰)
        Debug.DrawLine(firePoint.position, targetPoint, Color.red, 0.5f);
    }
}
