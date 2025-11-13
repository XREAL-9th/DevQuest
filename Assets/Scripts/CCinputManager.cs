using UnityEngine;

public class CCinputManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraTransform;

    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 5f;      // 걷기 속도
    [SerializeField] private float runSpeed = 9f;       // 달리기 속도
    [SerializeField] private float gravity = -9.81f;    // 중력
    [SerializeField] private float jumpHeight = 1.2f;   // 점프 높이
    [SerializeField] private float doubleJumpThreshold = 0.25f;  // 이단 점프 입력 간격

    private Vector2 moveInput;
    private bool isShiftPressed = false;

    private float yVelocity;
    private float lastJumpTime = 0f;   // 마지막 점프 버튼 눌린 시간
    private bool canDoubleJump = false;

    private Players input;


    private void Awake()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        input = new Players();   // InputActionAsset 기반 생성
    }

    private void OnEnable()
    {
        input.Player.Enable();

        // ───────────── Move 입력 ─────────────
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // ───────────── Run 입력 (Shift) ─────────────
        input.Player.Run.performed += _ => isShiftPressed = true;
        input.Player.Run.canceled += _ => isShiftPressed = false;

        // ───────────── Jump 입력 ─────────────
        input.Player.Jump.performed += _ => HandleJumpInput();
    }

    private void OnDisable()
    {
        input.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        input.Player.Run.performed -= _ => isShiftPressed = true;
        input.Player.Run.canceled -= _ => isShiftPressed = false;
        input.Player.Jump.performed -= _ => HandleJumpInput();

        input.Player.Disable();
    }




    // ───────────────────────────────────────────
    //                  UPDATE
    // ───────────────────────────────────────────
    private void Update()
    {
        Vector3 camForward = cameraTransform ? cameraTransform.forward : Vector3.forward;
        Vector3 camRight = cameraTransform ? cameraTransform.right : Vector3.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * moveInput.y + camRight * moveInput.x;

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        // ───────────── 중력 처리 ─────────────
        if (controller.isGrounded && yVelocity < 0f)
        {
            yVelocity = -2f;
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }   
        // ───────────── 속도 결정 (걷기 vs 달리기) ─────────────
        float currentSpeed = (isShiftPressed && moveInput.y > 0) ? runSpeed : moveSpeed;

        Vector3 velocity = move * currentSpeed + Vector3.up * yVelocity;
        controller.Move(velocity * Time.deltaTime);
    }



    // ───────────────────────────────────────────
    //             JUMP / DOUBLE JUMP LOGIC
    // ───────────────────────────────────────────
    private void HandleJumpInput()
    {
        float now = Time.time;

        // 1단 점프: 땅에 있는 상태일 때
        if (controller.isGrounded)
        {
            DoJump();
            canDoubleJump = true;
            lastJumpTime = now;
            return;
        }

        // 이단 점프: 공중 + 시간 차이 짧을 때
        if (canDoubleJump && now - lastJumpTime <= doubleJumpThreshold)
        {
            DoJump();
            canDoubleJump = false;
        }

        lastJumpTime = now;
    }

    private void DoJump()
    {
        yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
