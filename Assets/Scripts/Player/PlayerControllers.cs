using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // 1. 싱글톤 인스턴스 선언
    public static PlayerController Instance { get; private set; }

    [Header("References")]
    public Transform cameraTransform; 
    

    [Header("Movement Settings")]
    public float walkSpeed = 3f;       
    public float runSpeed = 6f;        
    public float gravity = -9.81f;     
    public float jumpHeight = 1.2f;    
    public int maxJumpCount = 2;       

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 120f; 
    public float pitchLimit = 80f;        

    private CharacterController controller;
    private PlayerActions input;          
    private Vector2 moveInput;
    private float yVelocity;
    private int jumpCount;
    private float pitch;

    private void Awake()
    {
        // 2. 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

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
        bool isRunning = Keyboard.current.leftShiftKey.isPressed && moveInput.y > 0f;
        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 move = (right * moveInput.x + forward * moveInput.y).normalized;

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