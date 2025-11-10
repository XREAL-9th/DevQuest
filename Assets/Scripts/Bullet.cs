using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public float knockbackForce = 40f;

    void Start() => Destroy(gameObject, lifeTime);

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            // 1) 충돌 지점과 적의 질량중심을 이용해 "밖으로" 밀 방향 계산
            Rigidbody enemyRb = collision.rigidbody; // 맞은 쪽의 Rigidbody
            if (enemyRb != null)
            {
                // 첫 접촉점
                ContactPoint cp = collision.GetContact(0);
                // 적의 질량중심 -> 접촉점의 반대방향(=적을 밖으로 미는 방향)
                Vector3 dir = (enemyRb.worldCenterOfMass - cp.point).normalized;

                // 힘을 접촉 위치에서 가해주면 더 자연스럽게 밀림
                enemyRb.AddForceAtPosition(dir * knockbackForce, cp.point, ForceMode.Impulse);
            }
        }

        // 무엇이든 맞으면 총알 제거
        Destroy(gameObject);
    }
}
