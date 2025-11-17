using UnityEngine;
using TMPro;

public class FrameCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;

    private float deltaTime = 0f;
    private float updateTimer = 0f;
    private const float UpdateInterval = 0.5f;  // FPS 업데이트 간격

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (fpsText == null)
        {
            fpsText = GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        updateTimer += Time.unscaledDeltaTime;

        if (updateTimer >= UpdateInterval)
        {
            if (fpsText != null) 
            {
                float msec = deltaTime * 1000.0f;
                float fps = 1.0f / deltaTime;
                string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
                fpsText.text = text;  
            }
            
            updateTimer = 0f;
        }
    }
}
