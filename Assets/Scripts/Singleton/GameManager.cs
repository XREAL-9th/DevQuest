using System;
using UnityEngine;

public enum GameState
{
    None,
    Playing,
    GameOver,
    Win
}

namespace XREAL
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager singleton;
        public PlayerDataSO playerData;
        public GameState gameState = GameState.None;

        [SerializeField] private int winScore = 150;
        public int WinScore => winScore;

        public event Action OnGameOver;
        public event Action OnGameRestart;

        private void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void AddScore(int score)
        {
            playerData.Score += score;
            if (playerData.Score >= winScore)
            {
                WinGame();
            }
        }

        public void ResetScore()
        {
            playerData.Score = 0;
        }

        public void WinGame()
        {
            gameState = GameState.Win;
            UIManager.singleton.ShowWinPanel();
        }

        public void GameOver()
        {
            gameState = GameState.GameOver;
            UIManager.singleton.ShowGameOverPanel();
        }

        public void RestartGame()
        {
            gameState = GameState.None;
            UIManager.singleton.HideGameOverPanel();
            UIManager.singleton.HideWinPanel();
            ResetScore();
            OnGameRestart?.Invoke();
            gameState = GameState.Playing;
        }
    }
}