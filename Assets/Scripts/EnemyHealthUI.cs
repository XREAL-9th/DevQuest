using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Enemy enemy;     // Enemy 컴포넌트
    [SerializeField] private Slider slider;   // HP_Slider

    [Header("Position")]
    [SerializeField] private Transform target;   // 보통 Enemy의 transform
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2.0f, 0);

    [Header("Behavior")]
    [SerializeField] private bool hideWhenFull = true;  // 체력 만땅이면 숨길지

    private void Reset()
    {
        target = transform.root;
    }

    private void Awake()
    {
        if (target == null && enemy != null) target = enemy.transform;
    }

    private void LateUpdate()
    {
        if (enemy == null || slider == null || target == null) return;

        // 체력 비율 반영
        slider.value = enemy.Health01;

        // 만피일 때 숨김 처리
        if (hideWhenFull)
        {
            bool show = enemy.Health01 < 0.999f;
            if (slider.gameObject.activeSelf != show)
                slider.gameObject.SetActive(show);
        }

        // 머리 위 위치 유지
        transform.position = target.position + worldOffset;
    }
}
