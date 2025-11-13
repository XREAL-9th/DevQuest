using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public float knockbackForce = 40f;

    void Start() => Destroy(gameObject, lifeTime);

    // 콜리전/트리거 둘 다 커버하려면 두 개 다 넣어도 OK
    void OnTriggerEnter(Collider other) => HandleHit(other, other.attachedRigidbody);
    void OnCollisionEnter(Collision collision) => HandleHit(collision.collider, collision.rigidbody);

    void HandleHit(Component hit, Rigidbody rb)
    {
        // 1) 적 찾기 (부모/자식 어디든)
        var enemy = hit.GetComponentInParent<EnemyHitCounter>() ?? hit.GetComponent<EnemyHitCounter>();
        if (enemy != null)
        {
            Debug.Log($"[Bullet] Hit {enemy.name}");
            enemy.TakeHit(1);

            // 2) 넉백(옵션)
            if (rb != null)
            {
                Vector3 dir = (rb.worldCenterOfMass - transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }

            Destroy(gameObject);
            return;
        }

        // 적이 아니어도 뭔가에 닿으면 제거
        Destroy(gameObject);
    }
}
