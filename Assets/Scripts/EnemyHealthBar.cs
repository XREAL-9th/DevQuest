using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Health health;

    private int lastHP;

    void Awake()
    {
        if (!health)
            health = GetComponentInParent<Health>();
        lastHP = -1;
    }

    void Update()
    {
        if (!health || !fill) return;

        if (lastHP != health.CurrentHP)
        {
            float ratio = Mathf.Clamp01((float)health.CurrentHP / health.MaxHP);
            fill.fillAmount = ratio;
            lastHP = health.CurrentHP;
        }
    }
}