using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    [Header("����")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform firePoint; // �ѱ� ��ġ
    [SerializeField] private GameObject projectilePrefab;

    [Header("�߻� ����")]
    [SerializeField] private float projectileSpeed = 50f;
    [SerializeField] private float fireRate = 0.2f; // ���� �ӵ�
    [SerializeField] private bool automaticFire = false; // �ڵ�/�ܹ�

    [Header("��Ȯ��")]
    [SerializeField] private float spread = 0f; // ź���� (0 = �Ϻ��� ��Ȯ��)

    [Header("����Ʈ")]
    [SerializeField] private ParticleSystem muzzleFlash; // �ѱ� ȭ��
    [SerializeField] private AudioClip shootSound;

    private float nextFireTime = 0f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        // �ڵ� ���
        if (automaticFire && Input.GetMouseButton(0))
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
        // �ܹ� ���
        else if (!automaticFire && Input.GetMouseButtonDown(0))
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        // ȭ�� �߾�(ũ�ν����)���� ����ĳ��Ʈ
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        // ������ ���
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000f);
        }

        // �߻� ���� ��� (ź���� ����)
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        // ź���� �߰�
        if (spread > 0)
        {
            direction = AddSpread(direction, spread);
        }

        // �߻�ü ����
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        // �߻�ü�� �ӵ� �ο�
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        // ����Ʈ
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // ����
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    // ź���� �Լ�
    Vector3 AddSpread(Vector3 direction, float spreadAmount)
    {
        Vector3 spread = new Vector3(
            Random.Range(-spreadAmount, spreadAmount),
            Random.Range(-spreadAmount, spreadAmount),
            Random.Range(-spreadAmount, spreadAmount)
        );

        return (direction + spread).normalized;
    }

    // ����׿� - �߻� ���� ǥ��
    void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoint.position, firePoint.forward * 2f);
        }
    }
}