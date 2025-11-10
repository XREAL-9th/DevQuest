using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;              

    [Header("Detection (2-2)")]
    public float detectionRange = 10f;    // 감지 거리
    [Range(0, 180)] public float fieldOfView = 90f; // 정면 시야각

    [Header("Wander (2-1)")]
    public float wanderRadius = 8f;       // 배회 반경
    public float wanderDelay = 3f;        // 배회 간격

    private NavMeshAgent agent;
    private float wanderTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        wanderTimer = wanderDelay;
    }

    private void Start()
    {
        if (NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.applyRootMotion = false;
    }

    private void Update()
    {
        if (player == null || agent == null || !agent.isOnNavMesh) return;

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;
        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);

        // 플레이어가 정면 + 감지거리 내에 있을 때만 추적
        if (distance <= detectionRange && angle <= fieldOfView * 0.5f)
        {
            agent.stoppingDistance = 1f;
            agent.SetDestination(player.position);
        }
        else
        {
            Wander();
        }
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer > 0f) return;

        Vector3 random = transform.position + Random.insideUnitSphere * wanderRadius;
        if (NavMesh.SamplePosition(random, out var hit, wanderRadius + 2f, NavMesh.AllAreas))
        {
            agent.stoppingDistance = 0f;
            agent.SetDestination(hit.position);
        }

        wanderTimer = wanderDelay;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.6f, 1f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 left = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + left * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + right * detectionRange);
    }
}
