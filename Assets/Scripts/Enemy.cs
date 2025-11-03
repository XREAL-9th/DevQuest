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
    [SerializeField] private NavMeshAgent agent;

    [Header("Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float sightRange = 8f;   // 플레이어를 보는 거리
    [SerializeField] private float fov = 120f;        // 시야각 (정면만)
    [SerializeField] private float wanderRadius = 6f; // 배회 반경

    private Vector3 wanderTarget;

    public enum State
    {
        None,
        Idle,
        Chase,
        Attack
    }

    [Header("Debug")]
    public State state = State.None;
    public State nextState = State.None;

    private bool attackDone;
    private Transform player;

    private void Start()
    {
        state = State.None;
        nextState = State.Idle;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        PickNewWanderPoint();
    }

    private void Update()
    {
        //1. 상태 전환 조건
        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Idle:
                    // 플레이어가 시야 안에 있으면 Chase
                    if (PlayerInSight())
                    {
                        nextState = State.Chase;
                    }
                    else
                    {
                        // 움직이다가 목적지 도착하면 다시 랜덤 이동
                        if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        {
                            PickNewWanderPoint();
                        }
                    }
                    break;

                case State.Chase:
                    // 공격 사거리 들어오면 Attack
                    if (Vector3.Distance(transform.position, player.position) <= attackRange)
                        nextState = State.Attack;

                    // 시야에서 벗어나면 다시 Idle
                    if (!PlayerInSight())
                        nextState = State.Idle;

                    break;

                case State.Attack:
                    if (attackDone)
                    {
                        attackDone = false;
                        nextState = State.Idle;
                    }
                    break;
            }
        }

        //2. 상태 초기화
        if (nextState != State.None)
        {
            state = nextState;
            nextState = State.None;

            switch (state)
            {
                case State.Idle:
                    agent.isStopped = false;
                    PickNewWanderPoint();
                    break;

                case State.Chase:
                    agent.isStopped = false;
                    break;

                case State.Attack:
                    agent.isStopped = true;
                    Attack();
                    break;
            }
        }

        //3. 글로벌 업데이트
        if (state == State.Chase)
        {
            agent.SetDestination(player.position);
        }
    }

    private bool PlayerInSight()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > sightRange) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle < fov / 2f)
            return true;

        return false;
    }

    private void PickNewWanderPoint()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
            agent.SetDestination(wanderTarget);
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
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
}
