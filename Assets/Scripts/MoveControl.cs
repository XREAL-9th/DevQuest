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
    [SerializeField][Range(1f, 10f)] private float moveSpeed;
    [SerializeField][Range(1f, 10f)] private float jumpAmount;

    // ---- 이단점프 ----
    [SerializeField] private int maxJumpCount = 2;
    private int jumpCount = 0;

    // ---- 달리기 ----
    [SerializeField] private float runMultiplier = 2f;
    private bool isRunning = false;

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

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        state = State.None;
        nextState = State.Idle;
        stateTime = 0f;
    }

    private void Update()
    {
        stateTime += Time.deltaTime;
        CheckLanded();

        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Idle:
                    if (landed)
                    {
                        jumpCount = 0;
                        if (Input.GetKeyDown(KeyCode.Space))
                            nextState = State.Jump;
                    }
                    break;

                case State.Jump:
                    if (!landed && Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
                        nextState = State.Jump;

                    if (landed && stateTime > 0.05f)
                        nextState = State.Idle;
                    break;
            }
        }

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

        UpdateInput(); // 이동은 Update에서
    }

    private void CheckLanded()
    {
        var center = col.bounds.center;
        var origin = new Vector3(center.x, center.y - ((col.height - 1f) / 2 + 0.15f), center.z);
        landed = Physics.CheckSphere(origin, 0.45f, 1 << 3, QueryTriggerInteraction.Ignore);
    }

    private void UpdateInput()
    {
        forward = transform.forward;
        right = transform.right;

        var direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += forward;
        if (Input.GetKey(KeyCode.A)) direction += -right;
        if (Input.GetKey(KeyCode.S)) direction += -forward;
        if (Input.GetKey(KeyCode.D)) direction += right;

        direction.Normalize();

        bool pressW = Input.GetKey(KeyCode.W);
        bool pressShift = Input.GetKey(KeyCode.LeftShift);

        isRunning = pressW && pressShift;
        float speed = isRunning ? moveSpeed * runMultiplier : moveSpeed;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}
