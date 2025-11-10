using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool instance;
    
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float respawnDelay = 5f; // 5초 텀
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    private Dictionary<GameObject, Vector3> enemyStartPositions = new Dictionary<GameObject, Vector3>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // 풀 초기화
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            pool.Enqueue(enemy);
        }
    }

    // 풀에서 Enemy 가져오기
    public GameObject GetEnemy(Vector3 position)
    {
        GameObject enemy;

        if (pool.Count > 0)
        {
            enemy = pool.Dequeue();
        }
        else
        {
            enemy = Instantiate(enemyPrefab);
        }

        enemy.transform.position = position;
        enemy.SetActive(true);
        
        // 처음 위치 저장
        if (!enemyStartPositions.ContainsKey(enemy))
            enemyStartPositions[enemy] = position;

        return enemy;
    }

    // Enemy를 풀로 반환 (5초 후 처음 위치에서 재활성화)
    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        
        // 5초 후 처음 위치에서 자동으로 재활성화
        StartCoroutine(RespawnEnemy(enemy));
    }

    private IEnumerator RespawnEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(respawnDelay);
        
        // 처음 위치에서 다시 나타남
        if (enemyStartPositions.ContainsKey(enemy))
        {
            enemy.transform.position = enemyStartPositions[enemy];
        }
        
        enemy.SetActive(true);
    }
}