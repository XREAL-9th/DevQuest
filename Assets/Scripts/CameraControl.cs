using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField][Range(1f, 20f)] private float sensitivity = 10f;

    private Players input;
    private Vector2 lookDelta;
    private float mouseX, mouseY;
    private Transform playerTransform;

    private void Awake()
    {
        input = new Players();
    }

    private void OnEnable()
    {
        input.Player.Enable();

        // 마우스 delta 입력 읽기
        input.Player.Look.performed += ctx => lookDelta = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += _ => lookDelta = Vector2.zero;
    }

    private void OnDisable()
    {
        input.Player.Look.performed -= ctx => lookDelta = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled -= _ => lookDelta = Vector2.zero;

        input.Player.Disable();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerTransform = transform.parent;
    }

    private void FixedUpdate()
    {
        float deltaX = lookDelta.x * sensitivity * Time.fixedDeltaTime;
        float deltaY = lookDelta.y * sensitivity * Time.fixedDeltaTime;

        mouseX += deltaX;
        playerTransform.rotation = Quaternion.Euler(0f, mouseX, 0f);

        mouseY += deltaY;
        mouseY = Mathf.Clamp(mouseY, -75f, 75f);
        transform.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);
    }
}
