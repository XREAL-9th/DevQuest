using UnityEngine;
using UnityEngine.UI;

public class HealthBarWorld : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, 0);
    public Image fill;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (!target) return;
        transform.position = target.position + offset;
        if (cam) transform.forward = cam.transform.forward;
        // ⚠️ 여기서 fill.fillAmount를 1로 재설정하는 코드가 있으면 절대 안 됩니다.
    }

    public void SetRatio(float r)
    {
        if (!fill)
        {
            Debug.LogWarning("[HPBAR] fill is NULL (Fill Image 연결 필요)");
            return;
        }
        float v = Mathf.Clamp01(r);
        fill.fillAmount = v;
        // 디버깅 원하면 아래 주석 해제
        // Debug.Log($"[HPBAR] SetRatio {v}");
    }
}
