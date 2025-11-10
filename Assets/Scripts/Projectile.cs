using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float speed = 1f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector3 velocity)
    {
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
        else
        {
            transform.position += velocity * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);  // 충돌하면 삭제
    }
}
