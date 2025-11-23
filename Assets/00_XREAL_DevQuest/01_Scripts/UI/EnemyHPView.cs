using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XREAL
{
    public class EnemyHPView : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;
        [SerializeField] private Slider enemyHp;
        [SerializeField] private TMP_Text enemyHpText;

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
            enemyHp.value = enemy.CurrentHp / 100f;
            enemyHpText.text = enemy.CurrentHp.ToString();
            enemy.OnHpChanged += HandleHpChanged;
        }

        private void HandleHpChanged(float hp)
        {
            enemyHp.value = hp / 100f;
            enemyHpText.text = hp.ToString();
        }
    }
}