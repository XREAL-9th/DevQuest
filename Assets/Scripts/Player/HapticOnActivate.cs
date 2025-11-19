using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace XREAL
{
    public class HapticOnActivate : MonoBehaviour
    {
        private SimpleHapticFeedback simpleHapticFeedback;

        [Header("Haptic Settings")]
        public float amplitude = 0.7f;
        public float duration = 0.1f;

        void Awake()
        {
            TryGetComponent<SimpleHapticFeedback>(out simpleHapticFeedback);
        }


        public void TriggerHaptic(ActivateEventArgs args)
        {
            var interactor = args.interactorObject as XRBaseInteractor;

            simpleHapticFeedback.hapticImpulsePlayer = interactor.gameObject.GetComponentInParent<HapticImpulsePlayer>();
            simpleHapticFeedback.hapticImpulsePlayer.SendHapticImpulse(amplitude, duration, 0f);
        }

    }
}