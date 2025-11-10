using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Header("Preset Fields")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject splashFx;

    [Header("Settings")]
    [SerializeField] private float attackRange;

    public enum State
    {
        None,
        Idle,
        Attack,
        Patrol,
        Chase,
        Charge
    }

    [Header("Debug")]
    public State state = State.None;
    public State nextState = State.None;

    private bool attackDone;

    [Header("Movement (2-1 & 2-2)")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private float wanderRadius = 20f;
    [SerializeField] private float wanderInterval = 5f;
    [SerializeField] private float idlePause = 1f;

    [Header("View (2-2)")]
    [SerializeField, Range(0f, 180f)] private float viewAngle = 30f;
    [SerializeField] private float viewDistance = 15f;
    private Vector3 originPosition;

    private float nextWanderTime;
    private Vector3 lastWanderPoint;

    [Header("Charge")]
    [SerializeField] private float chargeRange = 8f;             // 돌진을 시작할 최대 거리
    [SerializeField] private float chargeReadyTime = 0.4f;       // 텔레그래프(준비) 시간
    [SerializeField] private float chargeDuration = 0.7f;        // 실제 돌진 지속 시간
    [SerializeField] private float chargeSpeedMultiplier = 2.5f; // 돌진 중 속도 배수
    [SerializeField] private float chargeCooldown = 4f;          // 돌진 쿨다운
    private float nextChargeTime = 0f;
    private float savedAgentSpeed = 0f;
    private Coroutine chargeRoutine;

    private void Reset()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        originPosition = transform.position;

        state = State.None;
        nextState = State.Idle;
    }

    private void Update()
    {
        //1. 스테이트 전환 상황 판단
        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Idle:
                    if (PlayerVisibleInFront(out float dIdle))
                    {
                        // 우선순위: 근접공격 > 돌진 > 추격
                        if (dIdle <= attackRange)
                            nextState = State.Attack;
                        else if (dIdle <= chargeRange && Time.time >= nextChargeTime)
                            nextState = State.Charge;
                        else
                            nextState = State.Chase;
                    }
                    else
                    {
                        if (Time.time >= nextWanderTime)
                            nextState = State.Patrol;
                    }

                    // 기존의 근접 감지(레이어 6 사용) 유지
                    if (Physics.CheckSphere(transform.position, attackRange, 1 << 6, QueryTriggerInteraction.Ignore))
                        nextState = State.Attack;
                    break;

                case State.Attack:
                    if (attackDone)
                    {
                        nextState = State.Idle;
                        attackDone = false;
                    }
                    break;

                case State.Patrol:
                    if (PlayerVisibleInFront(out float dPatrol))
                    {
                        if (dPatrol <= attackRange)
                            nextState = State.Attack;
                        else if (dPatrol <= chargeRange && Time.time >= nextChargeTime)
                            nextState = State.Charge;
                        else
                            nextState = State.Chase;
                    }
                    break;

                case State.Chase:
                    if (!PlayerVisibleInFront(out _))
                        nextState = State.Idle;
                    else if (PlayerVisibleInFront(out float dChase))
                    {
                        if (dChase <= attackRange)
                            nextState = State.Attack;
                        else if (dChase <= chargeRange && Time.time >= nextChargeTime)
                            nextState = State.Charge;
                    }
                    break;

                case State.Charge:
                    // Charge는 코루틴이 끝날 때 nextState를 셋한다.
                    break;

                    //insert code here...
            }
        }

        //2. 스테이트 초기화
        if (nextState != State.None)
        {
            // Charge 중 다른 상태로 넘어가면 코루틴 정리
            if (state == State.Charge && chargeRoutine != null)
            {
                StopCoroutine(chargeRoutine);
                EndCharge();
            }

            state = nextState;
            nextState = State.None;

            switch (state)
            {
                case State.Idle:
                    if (agent != null)
                    {
                        agent.isStopped = false;
                        agent.ResetPath();
                    }
                    if (animator != null) animator.SetFloat("speed", 0f);
                    nextWanderTime = Time.time + idlePause;
                    break;

                case State.Attack:
                    Attack();
                    break;

                case State.Patrol: // Wander 시작
                    PickAndSetWanderPoint();
                    if (animator != null) animator.SetFloat("speed", 0.5f);
                    break;

                case State.Chase:
                    if (animator != null) animator.SetFloat("speed", 0.7f);
                    break;

                case State.Charge:
                    StartCharge();
                    break;

                    //insert code here...
            }
        }

        //3. 글로벌 & 스테이트 업데이트
        switch (state)
        {
            case State.Patrol:
                if (agent != null && !agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance + 0.1f)
                {
                    nextState = State.Idle;
                }
                break;

            case State.Chase:
                if (agent != null && player != null)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
                break;
        }

        //insert code here...
    }

    private bool PlayerVisibleInFront(out float distance)
    {
        distance = Mathf.Infinity;
        if (player == null) return false;

        Vector3 toPlayer = player.position - transform.position;
        distance = toPlayer.magnitude;

        // 각도 판정
        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
        if (angle > viewAngle) return false;

        // 거리 판정
        if (distance > viewDistance) return false;

        return true;
    }

    private void PickAndSetWanderPoint()
    {
        if (agent == null) return;

        Vector3 randomDir = Random.insideUnitSphere * wanderRadius + originPosition;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            lastWanderPoint = hit.position;
            agent.isStopped = false;
            agent.SetDestination(lastWanderPoint);
            nextWanderTime = Time.time + UnityEngine.Random.Range(wanderInterval * 0.8f, wanderInterval * 1.2f);
        }
        else
        {
            nextWanderTime = Time.time + 1.0f;
        }
    }

    private void StartCharge()
    {
        if (agent == null)
        {
            nextState = State.Idle;
            return;
        }

        savedAgentSpeed = agent.speed;
        agent.ResetPath();
        agent.isStopped = true; // 텔레그래프 동안 멈춤

        if (animator != null) animator.SetTrigger("chargeReady");

        // 돌진 코루틴 시작
        if (chargeRoutine != null) StopCoroutine(chargeRoutine);
        chargeRoutine = StartCoroutine(DoCharge());
    }

    private IEnumerator DoCharge()
    {
        // 텔레그래프
        yield return new WaitForSeconds(chargeReadyTime);

        if (agent == null)
        {
            nextState = State.Idle;
            yield break;
        }

        // 돌진 시작
        agent.isStopped = false;
        agent.speed = savedAgentSpeed * chargeSpeedMultiplier;
        if (animator != null) animator.SetBool("charging", true);

        // 돌진 목표점: 현재 플레이어 위치로 고정 (유도 돌진을 원하면 매 프레임 SetDestination 갱신)
        Vector3 target = player != null ? player.position : transform.position + transform.forward * 5f;
        agent.SetDestination(target);

        float endTime = Time.time + chargeDuration;
        while (Time.time < endTime)
        {
            if (player && Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                 break;
             }
            yield return null;
        }

        EndCharge();

        nextChargeTime = Time.time + chargeCooldown;
        nextState = State.Idle;
    }

    private void EndCharge()
    {
        if (agent != null)
        {
            agent.speed = savedAgentSpeed;
            agent.ResetPath();
            agent.isStopped = false;
        }
        if (animator != null) animator.SetBool("charging", false);
        chargeRoutine = null;
    }

    private void Attack()
    {
        animator.SetTrigger("attack");
    }

    public void InstantiateFx()
    {
        Instantiate(splashFx, transform.position, Quaternion.identity);
    }

    public void WhenAnimationDone()
    {
        attackDone = true;
    }


    private void OnDrawGizmosSelected()
    {
        //Gizmos를 사용하여 공격 범위를 Scene View에서 확인할 수 있게 합니다. (인게임에서는 볼 수 없습니다.)
        //해당 함수는 없어도 기능 상의 문제는 없지만, 기능 체크 및 디버깅을 용이하게 합니다.
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
}