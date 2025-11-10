using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject player;

    private float spawnXRange = 30f;
    private float spawnZRange = 30f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 10초마다 적 생성
        InvokeRepeating("SpawnEnemy", 10f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    {
        Vector3 randomPosition = GetRandomPosition();
        //Debug.Log("Spawning enemy at: " + randomPosition.ToString());
        Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
    }

    // 플레이어 주변의 랜덤 위치를 반환하는 함수
    Vector3 GetRandomPosition()
    {
        float randomX = player.transform.position.x + Random.Range(-spawnXRange, spawnXRange);
        float randomZ = player.transform.position.z + Random.Range(-spawnZRange, spawnZRange);
        return new Vector3(randomX, 0f, randomZ);
    }
}
