using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private float respawnTime = 10f;

    private Vector3 initialSpawnPosition;
    private GameObject currentMonsterInstance;

    void Start()
    {
        initialSpawnPosition = transform.position;
        SpawnMonster(initialSpawnPosition);
    }

    private void SpawnMonster(Vector3 pos)
    {
        currentMonsterInstance = Instantiate(monsterPrefab, transform.position, transform.rotation);
        Enemy enemy = currentMonsterInstance.GetComponent<Enemy>();
    }

    public void NotifyEnemyDeath()
    {
        currentMonsterInstance = null;
        StartCoroutine(RespawnCoroutine(initialSpawnPosition));
    }

    private IEnumerator RespawnCoroutine(Vector3 pos)
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnMonster(pos) ;
    }
}
