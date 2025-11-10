using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private NavMeshSurface map;
    [SerializeField] private float spawnInterval = 5f;

    private void Start()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Vector3 randomPosition = GetRandomNavMeshPosition();
            Spawn(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], randomPosition);
        }
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        Bounds bounds = map.navMeshData.sourceBounds;

        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero;
    }

    public void Spawn(GameObject prefab, Vector3 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }
}