using UnityEngine;

public class GameOver_Observer : MonoBehaviour, Observer
{
    private Hp_Subject subject = null; 

    [SerializeField] 
    private GameObject gameOver = null; 

    public void Init(Hp_Subject _subject) 
    { 
        // Subject를 초기화해줍니다.
        this.subject = _subject; 
    } 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ObserverUpdate(float _myHp, float _enemyHp) 
    { 
        if(_myHp <= 0f) 
        { 
            Debug.Log("플레이어 체력 0 - 게임 오버"); 
            this.gameOver.SetActive(true);
            Time.timeScale = 0;
        }
    } 
}
