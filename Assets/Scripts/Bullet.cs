using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ParticleSystem hitEffect;  // 총알이 맞았을 때 효과
    private Rigidbody enemyRigidbody;
    private UnityEngine.AI.NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 총알이 적에 맞으면
        if(collision.gameObject.tag == "Enemy")
        {
            // 적이 맞았을때
            Debug.Log("Enemy가 총 맞음");
            // 효과 생성
            Instantiate(hitEffect, transform.position, Quaternion.identity);
            FindObjectOfType<ObjectPooling>().ReturnToPool(this.gameObject);

            /*
            // 적이 뒤로 밀리게
            if (enemyRigidbody != null)
            {
                enemyRigidbody.isKinematic = false;
                enemyRigidbody.AddForce(-collision.contacts[0].normal * 700f, ForceMode.Impulse);
            }*/
        }
        else
        {   // 총알이 다른데 맞으면 그냥 반환
            FindObjectOfType<ObjectPooling>().ReturnToPool(this.gameObject);
        }
    }
}
