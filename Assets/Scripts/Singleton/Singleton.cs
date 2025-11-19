using UnityEngine;

namespace XREAL
{
    public class Singleton : MonoBehaviour
    {
        private static Singleton Instance { get; set; }

        //Managers
        public static LevelManager Level { get; private set; }
        public static GameManager Game { get; private set; }


        [SerializeField] private LevelManager levelSingleton;
        [SerializeField] private GameManager gameSingleton;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Level = levelSingleton;
            Game = gameSingleton;
        }
    }
}