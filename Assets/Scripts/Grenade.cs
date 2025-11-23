using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Grenade : MonoBehaviour
{
    [Header("Grenade Settings")]
    public float explosionRadius = 3f;
    public float explosionDamage = 100f;
    public float fuseTime = 3f;
    public float respawnTime = 2f;

    [Header("Spawn Settings")]
    public Vector3 spawnPosition;
    public Quaternion spawnRotation;

    private bool isThrown = false;
    private float throwTimer = 0f;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private bool hasExploded = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // 시작 위치 저장
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        // 이벤트 등록
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        isThrown = false;
        throwTimer = 0f;
        hasExploded = false;
    }

    void OnReleased(SelectExitEventArgs args)
    {
        isThrown = true;
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    void Update()
    {
        if (isThrown && !hasExploded)
        {
            throwTimer += Time.deltaTime;

            if (throwTimer >= fuseTime)
            {
                Explode();
            }
        }
    }

    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;
        Debug.Log("수류탄 폭발!");

        // 범위 내 적들 찾기
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
        }

        // 재생성
        Invoke(nameof(Respawn), respawnTime);

        // 임시로 비활성화
        gameObject.SetActive(false);
    }

    void Respawn()
    {
        // 위치 리셋
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        // 물리 리셋
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;
        rb.useGravity = true;

        // 상태 리셋
        isThrown = false;
        throwTimer = 0f;
        hasExploded = false;

        // 다시 활성화
        gameObject.SetActive(true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}