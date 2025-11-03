using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveControl : MonoBehaviour
{
    [Header("Preset Fields")]
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private CapsuleCollider col;

    [Header("Settings")]
    [SerializeField][Range(1f, 10f)] private float moveSpeed = 5f;
    [SerializeField][Range(1f, 10f)] private float jumpAmount = 5f;
    [SerializeField][Range(1f, 3f)] private float runMultiplier = 1.8f;
    [SerializeField][Range(50f, 500f)] private float mouseSensitivity = 150f;
    [SerializeField] private Transform cameraTransform; // 플레이어 자식 카메라
    [SerializeField][Range(30f, 90f)] private float maxLookAngle = 80f;

    public enum State
    {
        None,
        Idle,
        Jump
    }

    [Header("Debug")]
    public State state = State.None;
    public State nextState = State.None;
    public bool landed = false;

    private float stateTime;
    private Vector3 forward, right;

    private int jumpCount = 0;
    private int maxJumpCount = 2;
    private float xRotation = 0f;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        state = State.None;
        nextState = State.Idle;
        stateTime = 0f;
        forward = transform.forward;
        right = transform.right;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMouseLook(); // LateUpdate에서 호출 → 끊김 방지

        stateTime += Time.deltaTime;
        CheckLanded();

        if (landed) jumpCount = 0;

        // 상태 전환 판단
        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Idle:
                    if (landed && Input.GetKey(KeyCode.Space))
                        nextState = State.Jump;
                    break;

                case State.Jump:
                    if (!landed && jumpCount < maxJumpCount && Input.GetKeyDown(KeyCode.Space))
                        nextState = State.Jump;
                    else if (landed)
                        nextState = State.Idle;
                    break;
            }
        }

        // 상태 초기화
        if (nextState != State.None)
        {
            state = nextState;
            nextState = State.None;

            switch (state)
            {
                case State.Jump:
                    var vel = rigid.linearVelocity;
                    vel.y = jumpAmount;
                    rigid.linearVelocity = vel;
                    jumpCount++;
                    break;
            }

            stateTime = 0f;
        }
    }


    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 플레이어 좌우 회전
        transform.Rotate(Vector3.up * mouseX);

        // 카메라 상하 회전 (로컬 기준)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        forward = transform.forward;
        right = transform.right;
    }

    private void FixedUpdate()
    {
        UpdateInput();
    }

    private void CheckLanded()
    {
        var center = col.bounds.center;
        var origin = new Vector3(center.x, center.y - ((col.height - 1f) / 2 + 0.15f), center.z);
        landed = Physics.CheckSphere(origin, 0.45f, 1 << 3, QueryTriggerInteraction.Ignore);
    }

    private void UpdateInput()
    {
        var direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += forward;
        if (Input.GetKey(KeyCode.A)) direction += -right;
        if (Input.GetKey(KeyCode.S)) direction += -forward;
        if (Input.GetKey(KeyCode.D)) direction += right;

        direction.Normalize();

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            currentSpeed *= runMultiplier;

        transform.Translate(currentSpeed * Time.deltaTime * direction, Space.World);
    }
}

