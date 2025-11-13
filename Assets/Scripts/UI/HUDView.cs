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

        void Start()
        {
            Initialize();
        }

        public void OnEnable()
        {
            GameManager.singleton.playerData.OnDataChanged += HandleDataChanged;
        }

        private void OnDisable()
        {
            GameManager.singleton.playerData.OnDataChanged -= HandleDataChanged;
        }

        private void Initialize()
        {
            playerHp.value = GameManager.singleton.playerData.CurrentHp / 100f;
            playerHpText.text = GameManager.singleton.playerData.CurrentHp.ToString();
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
                    scoreText.text = ((int)newValue).ToString() + " / " + GameManager.singleton.WinScore.ToString();
                    break;
            }
        }
    }
}
