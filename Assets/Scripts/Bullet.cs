using UnityEngine;
using UnityEngine.Pool;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            ShowVFX();
            collision.gameObject.GetComponent<Enemy>().TakeDamage(10f);
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

