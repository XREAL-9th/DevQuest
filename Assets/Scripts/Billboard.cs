using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    private void LateUpdate()
    {
        if (cam == null)
        {
            if (Camera.main == null) return;
            cam = Camera.main;
        }

        // 캔버스가 항상 카메라를 바라보게
        transform.forward = cam.transform.forward;
    }
}
