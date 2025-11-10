using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // 자기 자신 제거
        Destroy(gameObject);
    }

    // 트리거 콜라이더용 (옵션)
    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
