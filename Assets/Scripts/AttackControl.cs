using UnityEngine;
using UnityEngine.Pool;

public class AttackControl : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private Camera mainCamera;
    private ObjectPool<GameObject> bulletPool;

    private void Start()
    {
        mainCamera = Camera.main;

        bulletPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: (obj) =>
            {
                obj.SetActive(true);
                obj.transform.position = firePoint.position;
                obj.transform.rotation = firePoint.rotation;
            },
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 50
        );
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireBullet();
        }
    }

    private void FireBullet()
    {
        GameObject bullet = bulletPool.Get();
        bullet.GetComponent<Bullet>().SetPool(bulletPool);

        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.linearVelocity = Vector3.zero;
        bulletRigidbody.AddForce(firePoint.forward * 20f, ForceMode.VelocityChange);
    }
}
