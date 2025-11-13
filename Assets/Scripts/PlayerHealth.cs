using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;
    private int currentHP;


    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"💔 Player damaged: -{amount} → HP {currentHP}");

        if (currentHP <= 0)
        {
            GameManager.Instance.Lose();
        }
    }
}