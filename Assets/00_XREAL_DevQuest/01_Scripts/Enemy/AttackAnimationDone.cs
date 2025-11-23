using UnityEngine;

namespace XREAL
{
    public class AttackAnimationDone : MonoBehaviour
    {
        [SerializeField] private Enemy enemy;

        public void OnAnimationDone()
        {
            enemy.WhenAnimationDone();
        }
    }
}