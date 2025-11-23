using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Sword : MonoBehaviour
{
    [Header("Sword Settings")]
    public float minDamageSpeed = 1f; // 최소 데미지를 주는 속도
    public float maxDamageSpeed = 5f; // 최대 데미지를 주는 속도
    public float minDamage = 10f;
    public float maxDamage = 50f;
    public float damageRadius = 0.3f; // 타격 판정 범위

    [Header("Visual Feedback")]
    public TrailRenderer trailRenderer;
    public float trailMinSpeed = 2f; // 이 속도 이상일 때 궤적 표시

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool isHeld = false;

    // 이미 맞은 적 추적 (중복 타격 방지)
    private System.Collections.Generic.HashSet<Enemy> hitEnemies = new System.Collections.Generic.HashSet<Enemy>();
    private float resetHitTimer = 0f;
    private float resetHitDelay = 0.3f; // 0.3초마다 타격 가능

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // 궤적 효과 생성
        CreateTrailEffect();

        // 이벤트 등록
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void CreateTrailEffect()
    {
        if (trailRenderer == null)
        {
            trailRenderer = gameObject.AddComponent<TrailRenderer>();
            trailRenderer.time = 0.2f;
            trailRenderer.startWidth = 0.1f;
            trailRenderer.endWidth = 0.01f;
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));

            // 그라디언트 (하얀색 -> 투명)
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.cyan, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trailRenderer.colorGradient = gradient;

            trailRenderer.enabled = false;
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    void OnReleased(SelectExitEventArgs args)
    {
        isHeld = false;
    }

    void Update()
    {
        if (!isHeld) return;

        // 속도 계산
        float speed = rb.velocity.magnitude;

        // 궤적 효과 ON/OFF
        if (trailRenderer != null)
        {
            trailRenderer.enabled = speed >= trailMinSpeed;
        }

        // 타격 판정 리셋 타이머
        resetHitTimer += Time.deltaTime;
        if (resetHitTimer >= resetHitDelay)
        {
            hitEnemies.Clear();
            resetHitTimer = 0f;
        }

        // 충분히 빠르면 타격 판정
        if (speed >= minDamageSpeed)
        {
            CheckHit(speed);
        }
    }

    void CheckHit(float speed)
    {
        // 칼 끝부분 기준으로 범위 체크
        Vector3 tipPosition = transform.position + transform.up * 0.25f; // 칼 끝

        Collider[] colliders = Physics.OverlapSphere(tipPosition, damageRadius);

        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();

            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                // 속도에 따른 데미지 계산
                float normalizedSpeed = Mathf.Clamp01((speed - minDamageSpeed) / (maxDamageSpeed - minDamageSpeed));
                float damage = Mathf.Lerp(minDamage, maxDamage, normalizedSpeed);

                enemy.TakeDamage(damage);
                hitEnemies.Add(enemy);

                Debug.Log($"칼로 {damage:F1} 데미지! (속도: {speed:F1})");

                // 타격감 (선택사항)
                // 컨트롤러 진동 추가 가능
            }
        }
    }

    void OnDrawGizmos()
    {
        // 타격 판정 범위 시각화
        Gizmos.color = Color.cyan;
        Vector3 tipPosition = transform.position + transform.up * 0.25f;
        Gizmos.DrawWireSphere(tipPosition, damageRadius);
    }
}