using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Health")]
    [SerializeField][Range(1f, 100f)] private float maxHp = 100f;

    private float currentHp;
    private Vector3 startPosition;

    private void Start()
    {
        currentHp = maxHp;
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        UIManager.singleton.OnGameRestart += RestartGame;
    }

    private void OnDisable()
    {
        UIManager.singleton.OnGameRestart -= RestartGame;
    }

    public void TakeDamage(float damage)
    {
        UIManager.singleton.PlayVignetteEffect();
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        UIManager.singleton.ShowGameOverPanel();
    }

    private void RestartGame()
    {
        currentHp = maxHp;
        gameObject.transform.position = startPosition;
        UIManager.singleton.HideGameOverPanel();
    }
}
