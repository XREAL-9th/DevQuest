using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XREAL
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Volume volume;

        [Header("UI")]
        [SerializeField] private CanvasGroup gameOverPanel;
        [SerializeField] private CanvasGroup winPanel;
        [SerializeField] private TMP_Text fpsCounter;

        public static UIManager singleton;
        private Coroutine vignetteCoroutine;
        private CancellationTokenSource cancellationTokenSource;

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
            HideWinPanel();
        }

        void OnEnable()
        {
            cancellationTokenSource = new CancellationTokenSource();
            _ = UpdateFPSCounter(cancellationTokenSource.Token);
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
            gameOverPanel.alpha = 1;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void HideGameOverPanel()
        {
            gameOverPanel.alpha = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ShowWinPanel()
        {
            winPanel.alpha = 1;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void HideWinPanel()
        {
            winPanel.alpha = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void RestartGame()
        {
            Singleton.Game.RestartGame();
        }

        private async Awaitable UpdateFPSCounter(CancellationToken cancellationToken)
        {
            float updateInterval = 0.1f;
            float timer = 0f;

            while (!cancellationToken.IsCancellationRequested)
            {
                timer += Time.unscaledDeltaTime;

                if (timer >= updateInterval)
                {
                    float fps = 1.0f / Time.unscaledDeltaTime;
                    string fpsText = $"{(int)fps} FPS";

                    fpsCounter.text = fpsText;
                    timer = 0f;
                }

                await Awaitable.NextFrameAsync();
            }
        }
    }
}