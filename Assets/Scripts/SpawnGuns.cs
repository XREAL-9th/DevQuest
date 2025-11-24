using UnityEngine;

public class SpawnGuns : MonoBehaviour
{
    public GameObject gunPrefab;
    public GameObject player;

    private float spawnXRange = 30f;
    private float spawnZRange = 30f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnGun", 5f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnGun()
    {
        Vector3 randomPosition = GetRandomPosition();
        //Debug.Log("Spawning gun at: " + randomPosition.ToString());
        Instantiate(gunPrefab, randomPosition, Quaternion.identity);
    }

    Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-spawnXRange, spawnXRange);
        float randomZ = Random.Range(-spawnZRange, spawnZRange);
        return new Vector3(randomX, 0f, randomZ);
    }
}