using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] monsterVariants;   // Variant 배열
    public int spawnCount;
    public Transform environment;          // Environment 프리팹 Transform
                                           // (씬에 배치된 Environment 오브젝트 드래그)

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        float sizeX = environment.localScale.x * 200f;
        float sizeZ = environment.localScale.z * 200f;

        for (int i = 0; i < spawnCount; i++)
        {
            float randX = Random.Range(-sizeX/2f, sizeX/2f);
            float randZ = Random.Range(-sizeZ/2f, sizeZ/2f);

            Vector3 randomPos = transform.position + new Vector3(randX, 10f, randZ);
            // Y를 의도적으로 10f~20f 정도로 높여서 공중에서 Raycast

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 50f))
            {
                Vector3 spawnPos = hit.point; // 바닥에 닿는 위치
                int r = Random.Range(0, monsterVariants.Length);
                Instantiate(monsterVariants[r], spawnPos, Quaternion.identity);
            }
        }
    }

}