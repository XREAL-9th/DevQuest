using UnityEngine;
using UnityEngine.UI;

public class GrenadeRangeUI : MonoBehaviour
{
    [Header("UI - Inspector에서 연결")]
    public Canvas worldCanvas;
    public Image rangeCircle;
    public Text rangeText;

    private Grenade grenade;

    void Start()
    {
        grenade = GetComponent<Grenade>();

        // 범위 텍스트 업데이트
        if (rangeText != null && grenade != null)
        {
            rangeText.text = $"{grenade.explosionRadius:F1}m";
        }
    }

    void Update()
    {
        // 카메라를 향하도록 (선택사항)
        if (Camera.main != null && rangeText != null)
        {
            rangeText.transform.rotation = Quaternion.LookRotation(
                rangeText.transform.position - Camera.main.transform.position);
        }
    }
}