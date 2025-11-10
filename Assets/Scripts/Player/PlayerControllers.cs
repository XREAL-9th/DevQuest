using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; 

    [Header("Movement Settings")]
    public float walkSpeed = 3f;       
    public float runSpeed = 6f;        // 달리기 속도
    public float gravity = -9.81f;     
    public float jumpHeight = 1.2f;    
    public int maxJumpCount = 2;       

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 120f; // 마우스 감도
    public float pitchLimit = 80f;        

    private CharacterController controller;
    private PlayerActions input;          
    private Vector2 moveInput;
    private float yVelocity;
    private int jumpCount;
    private float pitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new PlayerActions(); 
    }

    private void OnEnable()
    {
        input.Players.Enable();
        input.Players.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Players.Move.canceled += ctx => moveInput = Vector2.zero;
        input.Players.Jump.performed += _ => TryJump();
    }

    private void OnDisable()
    {
        input.Players.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;                   
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        // shift + W 달리기
        bool isRunning = Keyboard.current.leftShiftKey.isPressed && moveInput.y > 0f;
        float speed = isRunning ? runSpeed : walkSpeed;

        // 이동 방향 계산
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 move = (right * moveInput.x + forward * moveInput.y).normalized;

        // 중력 및 점프 처리
        if (controller.isGrounded)
        {
            if (yVelocity < 0f)
                yVelocity = -2f;
            jumpCount = 0; 
        }

        yVelocity += gravity * Time.deltaTime;
        Vector3 velocity = move * speed + Vector3.up * yVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (cameraTransform == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * Time.deltaTime;

        pitch -= mouseDelta.y;
        pitch = Mathf.Clamp(pitch, -pitchLimit, pitchLimit);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        transform.Rotate(Vector3.up * mouseDelta.x);
    }

    private void TryJump()
    {
        if (jumpCount < maxJumpCount)
        {
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpCount++;
        }
    }
}
