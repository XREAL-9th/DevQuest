using System;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    [Header("Preset Fields")]
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private CapsuleCollider col;
    
    [Header("Settings")]
    [SerializeField][Range(1f, 10f)] private float moveSpeed;
    [SerializeField][Range(5f, 15f)] private float moveRunSpeed;
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
    private Vector3 forward, right;
    private bool isNextDoubleJump = false;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        
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

        if (landed)
        {
            isNextDoubleJump = false;
        }

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
                            nextState = State.Jump;
                        }
                    }
                    break;
                case State.Jump:
                    if (landed && stateTime > 0.12f) //점프 직후 Idle로 오판되는 경우가 있어 대기시간 추가함.
                    {
                        nextState = State.Idle;
                    }
                    else if (!isNextDoubleJump && Input.GetKeyDown(KeyCode.Space))
                    {
                        // 이단점프
                        isNextDoubleJump = true;
                        SetDefaultRigid();
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
                    if (!isNextDoubleJump)
                    {
                        SetDefaultRigid();
                    }
                    break;
                //insert code here...
            }
            stateTime = 0f;
        }
        
        //3. 글로벌 & 스테이트 업데이트
        //insert code here...
    }

    private void SetDefaultRigid()
    {
        var vel = rigid.linearVelocity;
        vel.y = jumpAmount;
        rigid.linearVelocity = vel;
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
        var direction = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W)) direction += forward; //Forward
        if (Input.GetKey(KeyCode.A)) direction += -right; //Left
        if (Input.GetKey(KeyCode.S)) direction += -forward; //Back
        if (Input.GetKey(KeyCode.D)) direction += right; //Right
        
        direction.Normalize(); //대각선 이동(Ex. W + A)시에도 동일한 이동속도를 위해 direction을 Normalize

        // 어느 방향이던지 Shift 누르면 가속함. W만 가속하려면 조건 추가
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var speed = isRunning ? moveRunSpeed : moveSpeed;
        transform.Translate(speed * Time.deltaTime * direction); //Move
    }
}
