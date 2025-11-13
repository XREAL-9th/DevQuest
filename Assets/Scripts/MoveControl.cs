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
    [SerializeField][Range(1f, 5f)] private float runMultiplier;
    [SerializeField][Range(1f, 10f)] private float jumpAmount;
    [SerializeField][Range(1, 5)] private int maxJumpCount = 2;

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
    public int jumpCount = 0;

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
        //0. 글로벌 상황 판단
        stateTime += Time.deltaTime;
        CheckLanded();

        //1. 스테이트 전환 상황 판단
        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Idle:
                    if (landed && Input.GetKeyDown(KeyCode.Space))
                    {
                        nextState = State.Jump;
                    }
                    break;
                case State.Jump:
                    
                    if (!landed)
                    {
                        if (Input.GetKeyDown(KeyCode.Space) && jumpCount >0)
                        {
                            Jump();
                        }
                    }
                    else if (stateTime > 0.1f)
                    {
                        nextState = State.Idle;
                    }
                    break;
            }
        }

        //2. 스테이트 초기화
        if (nextState != State.None)
        {
            state = nextState;
            nextState = State.None;
            switch (state)
            {
                case State.Idle:
                    jumpCount = maxJumpCount;
                    break;
                case State.Jump:
                    Jump();
                    break;
            }
            stateTime = 0f;
        }
    }

    private void FixedUpdate()
    {
        UpdateInput();
    }

    private void Jump()
    {
        var vel = rigid.linearVelocity;
        vel.y = jumpAmount;
        rigid.linearVelocity = vel;

        jumpCount--;
        landed = false;
    }

    private void CheckLanded()
    {
        //발 위치에 작은 구를 하나 생성한 후, 그 구가 땅에 닿는지 검사한다.
        //1 << 3은 Ground의 레이어가 3이기 때문, << 는 비트 연산자
        var center = col.bounds.center;
        var origin = new Vector3(center.x, center.y - ((col.height - 1f) / 2 + 0.15f), center.z);
        landed = Physics.CheckSphere(origin, 0.45f, 1 << 3, QueryTriggerInteraction.Ignore);
    }

    private void UpdateInput()
    {
        forward = transform.forward;
        right = transform.right;

        var direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += forward; //Forward
        if (Input.GetKey(KeyCode.A)) direction += -right; //Left
        if (Input.GetKey(KeyCode.S)) direction += -forward; //Back
        if (Input.GetKey(KeyCode.D)) direction += right; //Right

        bool hasInput = direction != Vector3.zero;
        if (hasInput)
        {
            direction.Normalize(); //대각선 이동(Ex. W + A)시에도 동일한 이동속도를 위해 direction을 Normalize
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && hasInput;
        float speed = isRunning ? runMultiplier * moveSpeed : moveSpeed;

        Vector3 newVelocity = direction * speed;
        newVelocity.y = rigid.linearVelocity.y;
        rigid.linearVelocity = newVelocity;
    }
}
