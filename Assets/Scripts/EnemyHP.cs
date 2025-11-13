using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] 
    private Image hpBar = null; 

    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // 적 체력바가 항상 플레이어를 향하도록 설정
        if(player != null)
        {
            Vector3 direction = transform.position - player.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 rotation = lookRotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }
    }

    public void SetHpBar(float _enemyHp)
    {
        this.hpBar.fillAmount = _enemyHp / 50f; 
    }
}
