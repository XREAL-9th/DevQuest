using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Win Condition (Kill n)")]
    public int targetKillCount = 5;

    [Header("Runtime")]
    public int score;
    public int killCount;
    public int totalEnemies;
    public int aliveEnemies;

    public bool isGameOver;
    public bool isWin;

    public event Action OnWin;
    public event Action OnLose;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (totalEnemies == 0)
        {
            totalEnemies = FindObjectsOfType<EnemyHitCounter>().Length; // 네 현재 적 컴포넌트 기준
            aliveEnemies = totalEnemies;
        }
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
    }

    public void ReportEnemyDied()
    {
        if (isGameOver) return;
        killCount++;
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);

        if (killCount >= targetKillCount) Win();
    }

    public void Win()
    {
        if (isGameOver) return;
        isGameOver = true; isWin = true;
        OnWin?.Invoke();
        Time.timeScale = 0f;
    }

    public void Lose()
    {
        if (isGameOver) return;
        isGameOver = true; isWin = false;
        OnLose?.Invoke();
        Time.timeScale = 0f;
    }

    // 호환용(예전 코드 대비)
    public void GameOver() { Lose(); }
    public void GameOver(bool isWin_) { if (isWin_) Win(); else Lose(); }
}
