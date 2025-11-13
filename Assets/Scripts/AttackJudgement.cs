using UnityEngine;

public class AttackJudgement : MonoBehaviour
{
    [SerializeField] 
    private Hp_Subject hp_Subject = null; 

    [SerializeField] 
    private MyHp_Observer myHp_Observer = null;

    [SerializeField] 
    private GameOver_Observer gameOver_Observer = null;

    private EnemyHP enemyHP = null;


    // 플레이어의 체력
    private float originMyHp = 100f; 
    private float currentMyHp = 0f;

    // 적의 체력
    //private float originEnemyHp = 50f;
    private float currentEnemyHp = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.originMyHp = 100f; 
        this.currentMyHp = this.originMyHp;

        //this.originEnemyHp = 50f;
        //this.currentEnemyHp = this.originEnemyHp;

        // 옵저버 등록
        this.hp_Subject.RegisterObserver(this.myHp_Observer); 
        this.hp_Subject.RegisterObserver(this.gameOver_Observer);

        this.myHp_Observer.Init(this.hp_Subject);
        this.gameOver_Observer.Init(this.hp_Subject);

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other) 
    { 
        if(other.gameObject.CompareTag("Enemy")) 
        { 
            Debug.Log("플레이어가 적에게 공격받음 - 체력 10 감소");
            this.currentMyHp -= 10f; 
            Debug.Log("현재 플레이어 체력: " + this.currentMyHp);
            if(this.currentMyHp < 0f) 
            { 
                this.currentMyHp = 0f; 
            } 
            this.hp_Subject.Changed(this.currentMyHp, this.currentEnemyHp); 
        } 
        /*
        else if(this.gameObject.CompareTag("Enemy")&&other.gameObject.CompareTag("Bullet")) 
        { 
            Debug.Log("적이 총알에 맞음 - 체력 10 감소");
            this.currentEnemyHp -= 10f;
            Debug.Log("현재 적 체력: " + this.currentEnemyHp);
            if(this.currentEnemyHp < 0f) 
            { 
                this.currentEnemyHp = 0f; 
                this.gameObject.GetComponent<Enemy>().isDie = true;
            } 
            this.hp_Subject.Changed(this.currentMyHp, this.currentEnemyHp); 
            this.enemyHP.SetHpBar(this.currentEnemyHp);
        }*/
    }
}
