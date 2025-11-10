using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;      // 생성할 적 프리팹
    public int maxEnemyCount = 5;       // 최대 적 수
    public float spawnRadius = 15f;     // 생성 반경
    public float spawnInterval = 5f;    // 생성 주기 (초 단위)

    private int currentEnemyCount = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemyCount >= maxEnemyCount)
            return;

        Vector3 spawnPos = RandomPointOnNavMesh(transform.position, spawnRadius);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private static Vector3 RandomPointOnNavMesh(Vector3 origin, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = origin + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, NavMesh.AllAreas))
                return hit.position;
        }
        return origin;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
#endif
}
