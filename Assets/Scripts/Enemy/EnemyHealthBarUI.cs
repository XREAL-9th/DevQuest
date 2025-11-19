using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;

    private float maxHealth;

    public void SetMaxHealth(int maxHp)
    {
        maxHealth = maxHp;
    }

    public void UpdateHealth(int currentHp)
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHp / maxHealth;
        }
    }

    private void LateUpdate()
    {
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;
    }

}