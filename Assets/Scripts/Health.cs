using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHP = 30;
    int hp;

    void Awake() => hp = maxHP;

    public void TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0) Die();
    }

    void Die()
    {
        if (CompareTag("Enemy") && GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyDied();
        }
        Destroy(gameObject);
    }
}