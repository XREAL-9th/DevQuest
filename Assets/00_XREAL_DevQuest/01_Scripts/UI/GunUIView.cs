using TMPro;
using UnityEngine;

namespace XREAL
{
    public class GunUIView : MonoBehaviour
    {
        [SerializeField] private AttackControl attackControl;
        [SerializeField] private TMP_Text bulletText;

        void Start()
        {
            HandleBulletChanged(attackControl.CurrentBullet);
        }

        private void OnEnable()
        {
            attackControl.OnBulletChanged += HandleBulletChanged;
        }

        private void OnDisable()
        {
            attackControl.OnBulletChanged -= HandleBulletChanged;
        }

        private void HandleBulletChanged(int bullet)
        {
            bulletText.text = bullet.ToString() + " / " + attackControl.MaxBullet.ToString();
        }
    }
}