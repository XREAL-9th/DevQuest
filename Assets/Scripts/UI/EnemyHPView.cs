using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XREAL
{
    public class EnemyHPView : MonoBehaviour
    {
        private Enemy enemy;
        [SerializeField] private Slider playerHp;
        [SerializeField] private TMP_Text playerHpText;

        void Awake()
        {
            TryGetComponent<Enemy>(out enemy);
        }

        void Start()
        {
            Initialize();
        }

        void Update()
        {
            this.transform.LookAt(Camera.main.transform);
        }

        void OnDisable()
        {
            enemy.OnHpChanged -= HandleHpChanged;
        }

        private void Initialize()
        {
            playerHp.value = enemy.CurrentHp / 100f;
            playerHpText.text = enemy.CurrentHp.ToString();
            enemy.OnHpChanged += HandleHpChanged;
        }

        private void HandleHpChanged(float hp)
        {
            playerHp.value = hp / 100f;
            playerHpText.text = hp.ToString();
        }
    }
}