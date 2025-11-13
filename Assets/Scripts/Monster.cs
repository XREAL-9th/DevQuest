using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private NavMeshAgent nmAgent;
    private Transform target;

    [Header("Chase Settings")]
    public float chaseDistance = 15f;   // 플레이어를 감지하는 거리
    public float viewAngle = 120f;      // 시야각

    private bool isChasing = false;     // 추격 상태 여부

    private void Start()
    {
        nmAgent = GetComponent<NavMeshAgent>();

        // ✅ Player 태그 자동 탐색
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning($"⚠️ Monster '{name}' could not find any object tagged 'Player'.");
        }
    }

    private void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // ✅ 플레이어가 사정거리 안에 들어오면 추격 시작
        if (distance <= chaseDistance)
        {
            // 시야각 확인
            Vector3 dirToPlayer = (target.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToPlayer);

            if (angle <= viewAngle * 0.5f)
            {
                // 추격 모드 전환
                if (!isChasing)
                {
                    Debug.Log($"👁️ Monster '{name}' started chasing {target.name}");
                    isChasing = true;
                }

                nmAgent.isStopped = false;
                nmAgent.SetDestination(target.position);
                return;
            }
        }

        // 🧍 플레이어가 멀어지거나 시야 밖일 경우 추격 중단
        if (isChasing)
        {
            nmAgent.ResetPath();
            nmAgent.isStopped = true;
            isChasing = false;
            Debug.Log($"😴 Monster '{name}' stopped chasing {target.name}");
        }
    }
}
