using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Win, Lose }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private int targetEnemyCount = 5;
    [SerializeField] private float targetTime = 100f;

    public int TargetEnemyCount => targetEnemyCount;
    public float TargetTime => targetTime;

    [Header("Runtime Data")]
    public int enemiesDefeated { get; private set; }
    public float remainingTime { get; private set; }

    public event Action<int, int> OnEnemyDefeated;
    public event Action<float> OnTimeUpdated;
    public event Action<bool> OnGameOver;

    private bool isGameOver = false;
    private Coroutine timerRoutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        enemiesDefeated = 0;
        remainingTime = targetTime;
        isGameOver = false;

        OnEnemyDefeated?.Invoke(enemiesDefeated, targetEnemyCount);
        OnTimeUpdated?.Invoke(remainingTime);

        timerRoutine = StartCoroutine(GameTimer());
    }

    public void EnemyDefeated()
    {
        if (isGameOver) return;

        enemiesDefeated++;
        OnEnemyDefeated?.Invoke(enemiesDefeated, targetEnemyCount);

        if (enemiesDefeated >= targetEnemyCount)
            SetGameOver(true);
    }

    private System.Collections.IEnumerator GameTimer()
    {
        while (!isGameOver && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            OnTimeUpdated?.Invoke(remainingTime);
            yield return null;
        }

        if (!isGameOver)
            SetGameOver(false);
    }

    public void SetGameOver(bool win)
    {
        if (isGameOver) return;

        isGameOver = true;

        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        OnGameOver?.Invoke(win);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
