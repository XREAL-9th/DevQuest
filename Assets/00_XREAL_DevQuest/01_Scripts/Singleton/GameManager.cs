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
        public PlayerDataSO playerData;
        private AudioSource audioSource;
        public GameState gameState = GameState.None;

        [SerializeField] private int setGameLevel = 10;
        [SerializeField] private int winScore = 150;
        public int WinScore => winScore;

        private int initWinScore = 150;

        public event Action OnGameOver;
        public event Action OnGameRestart;

        void Awake()
        {
            TryGetComponent<AudioSource>(out audioSource);

            initWinScore = winScore;
        }

        public void AddScore(int score)
        {
            playerData.Score += score;

            if (playerData.Score >= winScore)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            playerData.Level++;

            if (playerData.Level >= setGameLevel)
            {
                WinGame();
                return;
            }

            audioSource.Play();
            winScore = playerData.Level * winScore;
            playerData.Score = 0;
        }

        public void ResetScore()
        {
            playerData.Score = 0;
            playerData.Level = 1;
            winScore = initWinScore;
        }

        public void WinGame()
        {
            gameState = GameState.Win;
            Singleton.UI.ShowWinPanel();
        }

        public void GameOver()
        {
            gameState = GameState.GameOver;
            Singleton.UI.ShowGameOverPanel();
        }

        public void RestartGame()
        {
            gameState = GameState.None;
            Singleton.UI.HideGameOverPanel();
            Singleton.UI.HideWinPanel();
            ResetScore();
            OnGameRestart?.Invoke();
            gameState = GameState.Playing;
        }
    }
}