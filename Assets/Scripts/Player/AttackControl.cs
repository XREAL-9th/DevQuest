using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace XREAL
{
    public class AttackControl : MonoBehaviour
    {
        public AudioClip reloadClip;
        public AudioClip fireClip;
        public AudioClip emptyClip;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField][Range(1f, 100f)] private float recoilForce = 100f;
        [SerializeField][Range(1, 100)] private int maxBullet = 30;
        public int MaxBullet => maxBullet;

        private Rigidbody rb;
        private AudioSource audioSource;
        private ObjectPool<GameObject> bulletPool;
        private int currentBullet = 0;
        public int CurrentBullet => currentBullet;

        public event Action<int> OnBulletChanged;

        private void Start()
        {
            TryGetComponent<AudioSource>(out audioSource);
            TryGetComponent<Rigidbody>(out rb);

            currentBullet = maxBullet;

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

        public void OnReload(InputValue value)
        {
            audioSource.PlayOneShot(reloadClip);
            currentBullet = maxBullet;
            OnBulletChanged?.Invoke(currentBullet);
        }

        public void FireBullet()
        {
            if (currentBullet <= 0)
            {
                audioSource.PlayOneShot(emptyClip);
                return;
            }

            audioSource.PlayOneShot(fireClip);

            rb.AddForce(-transform.forward * recoilForce, ForceMode.Impulse);

            GameObject bullet = bulletPool.Get();
            bullet.GetComponent<Bullet>().SetPool(bulletPool);

            Vector3 shootDirection = firePoint.forward;

            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bullet.transform.forward = shootDirection;
            bulletRigidbody.linearVelocity = shootDirection * 30f;

            currentBullet--;
            OnBulletChanged?.Invoke(currentBullet);
        }
    }
}