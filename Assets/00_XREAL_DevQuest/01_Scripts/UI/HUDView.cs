using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XREAL
{
    public class HUDView : MonoBehaviour
    {
        [SerializeField] private Slider playerHp;
        [SerializeField] private TMP_Text playerHpText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text levelText;

        void Start()
        {
            Initialize();
        }

        public void OnEnable()
        {
            Singleton.Game.playerData.OnDataChanged += HandleDataChanged;
        }

        private void OnDisable()
        {
            Singleton.Game.playerData.OnDataChanged -= HandleDataChanged;
        }

        private void Initialize()
        {
            playerHp.value = Singleton.Game.playerData.CurrentHp / 100f;
            playerHpText.text = Singleton.Game.playerData.CurrentHp.ToString();
            levelText.text = "레벨 " + Singleton.Game.playerData.Level.ToString();
        }

        private void HandleDataChanged(string fieldName, object newValue)
        {
            switch (fieldName)
            {
                case "CurrentHp":
                    playerHp.value = (float)newValue / 100f;
                    playerHpText.text = ((float)newValue).ToString();
                    break;
                case "Score":
                    scoreText.text = ((int)newValue).ToString() + " / " + Singleton.Game.WinScore.ToString();
                    break;
                case "Level":
                    levelText.text = "레벨 " + ((int)newValue).ToString();
                    break;
            }
        }
    }
}
