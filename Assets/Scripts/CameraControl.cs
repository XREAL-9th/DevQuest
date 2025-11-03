using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField][Range(1f, 20f)] private float sensitivity = 10f;
    private float mouseX, mouseY;
    private Transform playerTransform;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerTransform = transform.parent;
    }

    void LateUpdate()
    {
        // 1. 입력값 읽기 (프레임당)
        float dx = Input.GetAxis("Mouse X") * sensitivity;
        float dy = Input.GetAxis("Mouse Y") * sensitivity;

        // 2. 누적
        mouseX += dx;
        mouseY += dy;

        // 3. Pitch 제한
        mouseY = Mathf.Clamp(mouseY, -75f, 75f);

        // 4. 각도 wrap 방지 (0~360 유지)
        if (mouseX > 360f || mouseX < -360f)
            mouseX = Mathf.Repeat(mouseX, 360f);

        // 5. 회전 적용
        playerTransform.rotation = Quaternion.Euler(0, mouseX, 0);
        transform.localRotation = Quaternion.Euler(-mouseY, 0, 0);
    }
}
