using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public TMP_Text fpsText;

    private float deltaTime = 0f;

    void Update()
    {
        // 평균 델타타임 누적
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // FPS 계산
        float fps = 1.0f / deltaTime;

        // 출력
        fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";
    }
}
