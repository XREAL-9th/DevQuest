using UnityEngine;

namespace XREAL
{
    public class Singleton : MonoBehaviour
    {
        private static Singleton Instance { get; set; }

        //Managers
        public static LevelManager Level { get; private set; }
        public static GameManager Game { get; private set; }
        public static UIManager UI { get; private set; }

        [SerializeField] private LevelManager levelSingleton;
        [SerializeField] private GameManager gameSingleton;
        [SerializeField] private UIManager uiSingleton;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(this);
            Instance = this;

            Level = levelSingleton;
            Game = gameSingleton;
            UI = uiSingleton;
        }
    }
}