using UnityEngine;

namespace XREAL
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;

        private void Start()
        {
            SetGame();
            Singleton.Game.gameState = GameState.Playing;
        }

        private void OnEnable()
        {
            Singleton.Game.OnGameRestart += ResetGame;
        }

        private void OnDisable()
        {
            Singleton.Game.OnGameRestart -= ResetGame;
        }

        public void TakeDamage(float damage)
        {
            if (Singleton.Game.gameState != GameState.Playing)
            {
                return;
            }

            UIManager.singleton.PlayVignetteEffect();
            playerData.CurrentHp -= damage;
            if (playerData.CurrentHp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Singleton.Game.GameOver();
        }

        private void SetGame()
        {
            playerData.CurrentHp = playerData.MaxHp;
            playerData.StartPosition = this.transform.position;
            playerData.Score = 0;
        }

        private void ResetGame()
        {
            playerData.CurrentHp = playerData.MaxHp;
            gameObject.transform.position = playerData.StartPosition;
            playerData.Score = 0;
        }
    }
}