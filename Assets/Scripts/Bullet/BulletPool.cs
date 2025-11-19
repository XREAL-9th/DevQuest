using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [Header("Prefabs & Defaults")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private BulletData defaultBulletData; // 반드시 할당

    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public GameObject GetFromPool()
    {
        GameObject go = pool.Count > 0 ? pool.Dequeue() : Instantiate(bulletPrefab);

        // 안전 초기화
        var bullet = go.GetComponent<Bullet>();
        if (bullet.bulletData == null) bullet.bulletData = defaultBulletData;

        go.SetActive(true);
        return go;
    }

    public void ReturnToPool(GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);

        // 총알 물리/상태 리셋(필요 시)
        var rb = go.GetComponent<Rigidbody>();
        if (rb) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        pool.Enqueue(go);
    }
}
