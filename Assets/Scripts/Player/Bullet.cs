using UnityEngine;
using UnityEngine.Pool;

namespace XREAL
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private GameObject vfx;

        private float timer;
        private ObjectPool<GameObject> pool;

        public void SetPool(ObjectPool<GameObject> bulletPool)
        {
            pool = bulletPool;
        }

        private void OnEnable()
        {
            timer = 0f;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                ShowVFX();
                var enemy = other.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(10f);
            }
            ReturnToPool();
        }

        private void ShowVFX()
        {
            Instantiate(vfx, transform.position, transform.rotation);
        }

        private void ReturnToPool()
        {
            if (pool != null)
            {
                pool.Release(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}