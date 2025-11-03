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
        Chase
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
    private Vector3 originPosition;

    private float nextWanderTime;
    private Vector3 lastWanderPoint;

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
                    if (PlayerVisibleInFront())
                        nextState = State.Chase;
                    else
                    {
                        if (Time.time >= nextWanderTime)
                            nextState = State.Patrol;
                    }

                    if (Physics.CheckSphere(transform.position, attackRange, 1 << 6, QueryTriggerInteraction.Ignore))
                    {
                        nextState = State.Attack;
                    }
                    break;

                case State.Attack:
                    if (attackDone)
                    {
                        nextState = State.Idle;
                        attackDone = false;
                    }
                    break;

                case State.Patrol:
                    if (PlayerVisibleInFront())
                        nextState = State.Chase;
                    break;

                case State.Chase:
                    if (!PlayerVisibleInFront())
                        nextState = State.Idle;
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
                case State.Idle:
                    if (agent != null) agent.ResetPath();
                    if (animator != null) animator.SetFloat("speed", 0f);
                    nextWanderTime = Time.time + idlePause;
                    break;

                case State.Attack:
                    Attack();
                    break;

                case State.Patrol:
                    PickAndSetWanderPoint();
                    if (animator != null) animator.SetFloat("speed", 0.5f);
                    break;

                case State.Chase:
                    if (animator != null) animator.SetFloat("speed", 0.7f);
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
                    agent.SetDestination(player.position);
                }
                break;
        }

        //insert code here...
    }

    private bool PlayerVisibleInFront()
    {
        if (player == null) return false;

        Vector3 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
        return angle <= viewAngle && distance <= 10f;
    }

    private void PickAndSetWanderPoint()
    {
        if (agent == null) return;

        Vector3 randomDir = Random.insideUnitSphere * wanderRadius + originPosition;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            lastWanderPoint = hit.position;
            agent.SetDestination(lastWanderPoint);
            nextWanderTime = Time.time + UnityEngine.Random.Range(wanderInterval * 0.8f, wanderInterval * 1.2f);
        }
        else
        {
            nextWanderTime = Time.time + 1.0f;
        }
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