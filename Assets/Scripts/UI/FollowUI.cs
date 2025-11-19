using UnityEngine;

namespace XREAL
{
    public class FollowUI : MonoBehaviour
    {
        [SerializeField] private Transform targetCamera;
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private bool followRotation = true;
        [SerializeField] private Vector3 offset = Vector3.zero;

        private void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main?.transform;
            }
        }

        private void LateUpdate()
        {
            if (targetCamera == null) return;

            Vector3 targetPosition = targetCamera.position + targetCamera.TransformDirection(offset);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            if (followRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetCamera.rotation, followSpeed * Time.deltaTime);
            }
        }
    }
}