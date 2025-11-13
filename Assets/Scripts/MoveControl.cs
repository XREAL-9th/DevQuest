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

    //FSM(finite state machine)에 대한 더 자세한 내용은 세션 3회차에서 배울 것입니다!
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
    public bool moving = false;

    [Header("Jump Settings")]
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private float groundedResetDelay = 0.06f;
    private float groundedTime = 0f;
    private int jumpCount = 0;

    private float stateTime;
    private Vector3 forward, right;

    private float currentSpeed;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        currentSpeed = moveSpeed;
        state = State.None;
        nextState = State.Idle;
        stateTime = 0f;
        forward = transform.forward;
        right = transform.right;
    }

    private void Update()
    {
        //0. 글로벌 상황 판단
        stateTime += Time.deltaTime;
        CheckLanded();
        //insert code here...

        if (landed && rigid.linearVelocity.y <= 0f)
        {
            groundedTime += Time.deltaTime;
            if (groundedTime >= groundedResetDelay) jumpCount = 0;
        }
        else
        {
            groundedTime = 0f;
        }

        //1. 스테이트 전환 상황 판단
        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Idle:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (jumpCount < maxJumpCount)
                        {
                            nextState = State.Jump;
                        }
                    }
                    break;

                case State.Jump:
                    if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
                    {
                        nextState = State.Jump;
                    }
                    if (landed)
                    {
                        nextState = State.Idle;
                    }
                    break;
                    //insert code here...
            }
        }

        //2. 스테이트 초기화
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
                    //insert code here...
            }
            stateTime = 0f;
        }

        //3. 글로벌 & 스테이트 업데이트
        //insert code here...
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

        forward = transform.forward;
        right = transform.right;

        if (Input.GetKey(KeyCode.W)) direction += forward; //Forward
        if (Input.GetKey(KeyCode.A)) direction += -right; //Left
        if (Input.GetKey(KeyCode.S)) direction += -forward; //Back
        if (Input.GetKey(KeyCode.D)) direction += right; //Right

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            currentSpeed = moveSpeed * 2f;
        else
            currentSpeed = moveSpeed;

        direction = direction.normalized;

        // Rigidbody.MovePosition으로 이동
        Vector3 targetPos = rigid.position + direction * currentSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(targetPos);
    }
}