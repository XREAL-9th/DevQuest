using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    private float stateTime;
    private int jumpCount = 0; // 점프 횟수 카운트
    private int maxJumps = 2; // 최대 2번 점프

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
                    if (landed) 
                    {
                        if (Input.GetKey(KeyCode.Space))
                        {
                            jumpCount = 1;
                            nextState = State.Jump;
                        }
                    }
                    break;
                case State.Jump:
                    if (landed) 
                    {
                        jumpCount = 0; // 착지하면 점프 카운트 리셋
                        nextState = State.Idle;
                    }
                    // 공중에서 Space 누르면 이단 점프
                    else if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
                    {
                        jumpCount++;
                        var vel = rigid.linearVelocity;
                        vel.y = jumpAmount;
                        rigid.linearVelocity = vel;
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
                case State.Jump:
                    var vel = rigid.linearVelocity;
                    vel.y = jumpAmount;
                    rigid.linearVelocity = vel;
                    break;
            }
            stateTime = 0f;
        }
    }

    private void FixedUpdate()
    {
        UpdateInput();
    }

    private void CheckLanded() {
        //발 위치에 작은 구를 하나 생성한 후, 그 구가 땅에 닿는지 검사한다.
        //1 << 3은 Ground의 레이어가 3이기 때문, << 는 비트 연산자
        var center = col.bounds.center;
        var origin = new Vector3(center.x, center.y - ((col.height - 1f) / 2 + 0.15f), center.z);
        landed = Physics.CheckSphere(origin, 0.45f, 1 << 3, QueryTriggerInteraction.Ignore);
    }
    
    private void UpdateInput()
    {
        // [문제]
        // transform.Translate()는 Local 좌표계를 기준으로 움직이기 때문에
        // Player가 회전하면 모든 각도에서 WASD가 일관되지 않음
        // 예: 180도 회전 후 W를 누르면 뒤로 감
        
        // [원인]
        // transform.forward/right가 Player 회전에 따라 변하는데,
        // transform.Translate()가 이를 Local 좌표로 해석하면서 중복 적용됨
        
        // [해결]
        // Rigidbody.velocity를 직접 설정하여 
        // World 좌표계에서 절대적인 이동 방향을 적용
        
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        var direction = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W)) direction += forward; //Forward
        if (Input.GetKey(KeyCode.A)) direction += -right; //Left
        if (Input.GetKey(KeyCode.S)) direction += -forward; //Back
        if (Input.GetKey(KeyCode.D)) direction += right; //Right
        
        direction.Normalize();
        
        // 달리기 속도 조정
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            currentSpeed = moveSpeed * 2f; // Shift + W로 2배 속도
        }
        
        // ===== 변경 전 =====
        // transform.Translate(currentSpeed * Time.deltaTime * direction);
        
        // ===== 변경 후 =====
        // Rigidbody.velocity를 직접 설정 (Y축은 중력 유지)
        rigid.linearVelocity = new Vector3(
            direction.x * currentSpeed,
            rigid.linearVelocity.y,
            direction.z * currentSpeed
        );
    }
}