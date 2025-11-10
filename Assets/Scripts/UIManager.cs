using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Volume volume;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    public static UIManager singleton;

    public event Action OnGameRestart;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        HideGameOverPanel();
    }

    private Coroutine vignetteCoroutine;

    public void PlayVignetteEffect()
    {
        if (vignetteCoroutine != null)
        {
            StopCoroutine(vignetteCoroutine);
        }
        vignetteCoroutine = StartCoroutine(VignetteEffectCoroutine());
    }

    private System.Collections.IEnumerator VignetteEffectCoroutine()
    {
        Vignette vignette;
        volume.profile.TryGet<Vignette>(out vignette);

        float duration = 1f;
        float startValue = vignette.intensity.value;
        float targetUp = 0.5f;
        float targetDown = 0f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(startValue, targetUp, t / duration);
            yield return null;
        }
        vignette.intensity.value = targetUp;

        t = 0f;
        float upValue = vignette.intensity.value;
        while (t < duration)
        {
            t += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(upValue, targetDown, t / duration);
            yield return null;
        }
        vignette.intensity.value = targetDown;

        vignetteCoroutine = null;
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartGame()
    {
        OnGameRestart?.Invoke();
    }
}
