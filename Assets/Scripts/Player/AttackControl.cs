using UnityEngine;
using UnityEngine.Pool;

namespace XREAL
{
    public class AttackControl : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;

        private AudioSource audioSource;
        private Camera mainCamera;
        private ObjectPool<GameObject> bulletPool;

        private void Start()
        {
            mainCamera = Camera.main;
            TryGetComponent<AudioSource>(out audioSource);

            bulletPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(bulletPrefab),
                actionOnGet: (obj) =>
                {
                    obj.SetActive(true);
                    obj.transform.position = firePoint.position;
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
            audioSource.Play();

            GameObject bullet = bulletPool.Get();
            bullet.GetComponent<Bullet>().SetPool(bulletPool);

            Ray centerRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 targetPoint;
            if (Physics.Raycast(centerRay, out RaycastHit hitInfo, 1000f))
            {
                targetPoint = hitInfo.point;
            }
            else
            {
                targetPoint = centerRay.GetPoint(1000f);
            }

            Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bullet.transform.forward = shootDirection;
            bulletRigidbody.linearVelocity = shootDirection * 30f;
        }
    }
}