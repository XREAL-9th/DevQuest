using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // 적의 체력
    private float originEnemyHp = 50f;
    private float currentEnemyHp = 0f;

    private EnemyHP enemyHP = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.originEnemyHp = 50f;
        this.currentEnemyHp = this.originEnemyHp;

        if(this.gameObject.CompareTag("Enemy")){
            this.enemyHP = this.gameObject.GetComponentInChildren<EnemyHP>(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) 
    { 
        if(this.gameObject.CompareTag("Enemy")&&other.gameObject.CompareTag("Bullet")) 
        { 
            Debug.Log("적이 총알에 맞음 - 체력 10 감소");
            this.currentEnemyHp -= 10f;
            Debug.Log("현재 적 체력: " + this.currentEnemyHp);
            if(this.currentEnemyHp <= 0f) 
            { 
                this.currentEnemyHp = 0f; 
                this.gameObject.GetComponent<Enemy>().isDie = true;
            } 
            this.enemyHP.SetHpBar(this.currentEnemyHp);
        }
    }
}
