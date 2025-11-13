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
        Run,
        Jump,
        DoubleJump
    }
    
    [Header("Debug")]
    public State state = State.None;
    public State nextState = State.None;
    public bool landed = false;
    public bool moving = false;
    
    private float stateTime;
    private Vector3 forward, right;

    private ObjectPooling objectPooling; // 오브젝트풀링

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        objectPooling = FindObjectOfType<ObjectPooling>();
        
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
        CheckMoving();  // 이동 중인지 감지

        //1. 스테이트 전환 상황 판단
        if (nextState == State.None) 
        {
            switch (state) 
            {
                case State.Idle:
                    if (landed) 
                    {
                        if (Input.GetKeyDown(KeyCode.Space)) 
                        {
                            nextState = State.Jump;
                        }
                        // 이동 중이고 shift키 클릭 시 달리기
                        else if(moving&& Input.GetKey(KeyCode.LeftShift)) 
                        {
                            nextState = State.Run;
                        }
                    }
                    break;
                case State.Run:
                    if(!moving || !Input.GetKey(KeyCode.LeftShift)) 
                    {
                        nextState = State.Idle;
                    }
                    break;
                case State.Jump:
                    if (!landed && Input.GetKeyDown(KeyCode.Space)) 
                    {
                        nextState = State.DoubleJump;
                    }
                    else if (landed) 
                    {
                        nextState = State.Idle;
                    }
                    break;
                case State.DoubleJump:
                    if (landed) 
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
                case State.Jump:
                    var vel = rigid.linearVelocity;
                    vel.y = jumpAmount;
                    rigid.linearVelocity = vel;
                    break;
                
                case State.DoubleJump:
                    var vel2 = rigid.linearVelocity;
                    vel2.y = jumpAmount;
                    rigid.linearVelocity = vel2;
                    break;
                // 달리기
                case State.Run:
                    moveSpeed = 10f;
                    break;
                case State.Idle:
                    moveSpeed = 5f;
                    break;
               
            }
            stateTime = 0f;
        }
        
        //3. 글로벌 & 스테이트 업데이트
        //insert code here...


        // 투사체 발사
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = objectPooling.GetFromPool();
            // 플레이어 앞쪽에 총알 위치
            bullet.transform.position = transform.position + transform.TransformDirection(Vector3.forward) * 1.5f; 
            
            // 발사 방향 설정
            Vector3 dir;
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f))
            {
                dir = (hit.point - bullet.transform.position).normalized;
            }
            else
            {
                dir = Camera.main.transform.forward;
            }

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = Vector3.zero;
            bulletRb.angularVelocity = Vector3.zero;
            bulletRb.AddForce(dir*50, ForceMode.Impulse);

            GetComponent<AudioSource>().Play();
        }
    }

    private void FixedUpdate()
    {
        UpdateInput();
        //Debug.Log("Current State: " + state);
    }

    private void CheckLanded() {
        //발 위치에 작은 구를 하나 생성한 후, 그 구가 땅에 닿는지 검사한다.
        //1 << 3은 Ground의 레이어가 3이기 때문, << 는 비트 연산자
        var center = col.bounds.center;
        var origin = new Vector3(center.x, center.y - ((col.height - 1f) / 2 + 0.12f), center.z);
        landed = Physics.CheckSphere(origin, 0.40f, 1 << 3, QueryTriggerInteraction.Ignore);
    }

    private void CheckMoving()
    {
        moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }
    
    private void UpdateInput()
    {
        var direction = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W)) direction += forward; //Forward
        if (Input.GetKey(KeyCode.A)) direction += -right; //Left
        if (Input.GetKey(KeyCode.S)) direction += -forward; //Back
        if (Input.GetKey(KeyCode.D)) direction += right; //Right
        
        direction.Normalize(); //대각선 이동(Ex. W + A)시에도 동일한 이동속도를 위해 direction을 Normalize
        
        transform.Translate( moveSpeed * Time.deltaTime * direction); //Move
    }
}
