using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public Transform target;

    [Header("Detection Settings")]
    public float detectionDistance = 10f;
    public float detectionAngle = 60f;

    [Header("Patrol Settings")]
    public bool enablePatrol = true;
    public float patrolRadius = 10f;
    public float patrolInterval = 5f;

    private NavMeshAgent nmAgent;
    private Vector3 patrolCenter;
    private float patrolTimer;

    private void Start()
    {
        nmAgent = GetComponent<NavMeshAgent>();
        patrolCenter = transform.position;
        patrolTimer = patrolInterval;
    }

    private void Update()
    {
        if (target != null && IsPlayerVisible())
        {
            nmAgent.SetDestination(target.position);
        }
        else if (enablePatrol)
        {
            Patrol();
        }
        else
        {
            nmAgent.ResetPath(); // «√∑π¿ÃæÓ∞° æ» ∫∏¿Ã∞Ì º¯¬˚ ≤®¡Æ¿÷¿∏∏È ∏ÿ√„
        }
    }

    private void Patrol()
    {
        patrolTimer -= Time.deltaTime;

        if (nmAgent.remainingDistance < 0.5f || patrolTimer <= 0f)
        {
            Vector3 newPatrolPoint = GetRandomPatrolPoint();
            nmAgent.SetDestination(newPatrolPoint);
            patrolTimer = patrolInterval;
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 randomPoint = patrolCenter + new Vector3(randomCircle.x, 0, randomCircle.y);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    private bool IsPlayerVisible()
    {
        Vector3 directionToPlayer = target.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > detectionDistance)
            return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > detectionAngle / 2f)
            return false;

        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, out RaycastHit hit, detectionDistance))
        {
            if (hit.transform == target)
                return true;
        }

        return false;
    }
}
