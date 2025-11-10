using UnityEngine;
using UnityEngine.UI;

public class MyHp_Observer : MonoBehaviour, Observer
{
    private Hp_Subject subject = null; 

    [SerializeField] 
    private Image hpBar = null; 

    public void Init(Hp_Subject _subject) 
    { 
        // Subject를 초기화해줍니다.
        this.subject = _subject; 
    } 

    public void ObserverUpdate(float _myHp, float _enemyHp) 
    { 
        this.hpBar.fillAmount = _myHp / 100f; 
    } 
}
