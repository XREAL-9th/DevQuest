using UnityEngine;

public class EnemyHitCounter : MonoBehaviour
{
    [SerializeField] int maxHits = 3;  // n번
    int hitCount = 0;

    void OnTriggerEnter(Collider other)
    {
        // Player가 쏜 총알인지 확인
        if (other.CompareTag("PlayerBullet"))
        {
            hitCount++;
            Destroy(other.gameObject); // 총알 제거

            if (hitCount >= maxHits)
            {
                Destroy(gameObject); // 적 제거
            }
        }
        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
        }
    }
}
