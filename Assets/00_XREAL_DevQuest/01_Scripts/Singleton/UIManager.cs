using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XREAL
{
    public class UIManager : MonoBehaviour
    {
        public static event Action<float> FadeIn;
        public static event Action<float> FadeOut;

        [SerializeField] private Volume volume;

        [Header("UI")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject winPanel;
        private Coroutine vignetteCoroutine;
        private CancellationTokenSource cancellationTokenSource;

        private void Start()
        {
            HideGameOverPanel();
            HideWinPanel();
        }

        void OnEnable()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        void OnDisable()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

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
            FadeIn?.Invoke(1f);
        }

        public void HideGameOverPanel()
        {
            gameOverPanel.SetActive(false);
            FadeOut?.Invoke(1f);
        }

        public void ShowWinPanel()
        {
            winPanel.SetActive(true);
            FadeIn?.Invoke(1f);
        }

        public void HideWinPanel()
        {
            winPanel.SetActive(false);
            FadeOut?.Invoke(1f);
        }

        public void RestartGame()
        {
            Singleton.Game.RestartGame();
        }
    }
}