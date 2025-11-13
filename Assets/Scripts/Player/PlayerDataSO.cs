using System;
using UnityEngine;

namespace XREAL
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData", order = 1)]
    public class PlayerDataSO : ScriptableObject
    {
        public event Action<string, object> OnDataChanged;

        [Header("Health")]
        [SerializeField][Range(1f, 100f)] private float maxHp = 100f;
        public float MaxHp
        {
            get => maxHp;
            set
            {
                maxHp = value;
                OnDataChanged?.Invoke(nameof(MaxHp), value);
            }
        }
        [SerializeField] private float currentHp = 100f;
        public float CurrentHp
        {
            get => currentHp;
            set
            {
                currentHp = value;
                OnDataChanged?.Invoke(nameof(CurrentHp), value);
            }
        }

        [SerializeField] private Vector3 startPosition;
        public Vector3 StartPosition
        {
            get => startPosition;
            set
            {
                startPosition = value;
                OnDataChanged?.Invoke(nameof(StartPosition), value);
            }
        }

        [SerializeField] private int score = 0;
        public int Score
        {
            get => score;
            set
            {
                score = value;
                OnDataChanged?.Invoke(nameof(Score), value);
            }
        }
    }
}