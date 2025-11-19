using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int maxEnemyCount = 5;
    public float spawnRadius = 15f;
    public float spawnInterval = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (currentEnemyCount >= maxEnemyCount)
            return;

        Vector3 spawnPos = RandomPointOnNavMesh(transform.position, spawnRadius);

        // 1. 프리팹 생성
        GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // 2. NavMeshAgent 초기화 및 Warp (생성 위치 보정)
        if (enemyObj.TryGetComponent(out NavMeshAgent agent))
        {
            if (!agent.enabled)
                agent.enabled = true;

            agent.Warp(spawnPos); 
        }
        else
        {
            Debug.LogError($"[Spawner] Spawned enemy '{enemyObj.name}' has no NavMeshAgent! Cannot initialize.");
        }

        // 3. FSM 시작
        if (enemyObj.TryGetComponent(out Enemy enemyScript))
        {
            enemyScript.Invoke(nameof(Enemy.GoWander), 0.5f);
        }
    }

    private static Vector3 RandomPointOnNavMesh(Vector3 origin, float radius)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 randomPoint = origin + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, NavMesh.AllAreas))
                return hit.position;
        }

        Debug.LogWarning("[Spawner] Failed to find NavMesh point — returning origin.");
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