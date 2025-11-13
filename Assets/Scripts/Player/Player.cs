using UnityEngine;

namespace XREAL
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;

        private void Start()
        {
            SetGame();
            GameManager.singleton.gameState = GameState.Playing;
        }

        private void OnEnable()
        {
            GameManager.singleton.OnGameRestart += ResetGame;
        }

        private void OnDisable()
        {
            GameManager.singleton.OnGameRestart -= ResetGame;
        }

        public void TakeDamage(float damage)
        {
            if (GameManager.singleton.gameState != GameState.Playing)
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
            GameManager.singleton.GameOver();
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