using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;   
    public Transform player;         
    public int enemyCount = 3;       // 생성 수
    public float spawnRadius = 10f;  // 생성 반경

    private void Start()
    {
        if (enemyPrefab == null || player == null)
        {
            Debug.LogError("EnemySpawner: enemyPrefab 또는 player가 비어 있습니다.");
            return;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 pos = RandomPointOnNavMesh(transform.position, spawnRadius);
            GameObject e = Instantiate(enemyPrefab, pos, Quaternion.identity);

            var ai = e.GetComponent<EnemyAI>();
            if (ai != null) ai.player = player;

            if (e.GetComponent<NavMeshAgent>() == null)
            {
                var agent = e.AddComponent<NavMeshAgent>();
                agent.speed = 3.5f;
                agent.angularSpeed = 360f;
                agent.acceleration = 8f;
                agent.stoppingDistance = 1.2f;
            }
        }
    }

    private static Vector3 RandomPointOnNavMesh(Vector3 origin, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 random = origin + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(random, out var hit, radius, NavMesh.AllAreas))
                return hit.position;
        }
        return origin;
    }
}
