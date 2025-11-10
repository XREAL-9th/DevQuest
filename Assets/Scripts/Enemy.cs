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
    [SerializeField] private Transform player; // 플레이어 위치
    
    [Header("Settings")]
    [SerializeField] private float attackRange = 2f; // 공격 범위
    [SerializeField] private float chaseRange = 10f; // 추적 범위
    [SerializeField] private float patrolRange = 5f; // 순찰 범위
    [SerializeField] private int maxHealth = 100; // 최대 체력 100
    
    private NavMeshAgent navMeshAgent;
    private Vector3 patrolTarget;
    private int currentHealth;
    private bool isDead = false;
    
    public enum State 
    {
        None,
        Idle,
        Chase,
        Attack,
        Dead
    }
    
    [Header("Debug")]
    public State state = State.None;
    public State nextState = State.None;

    private bool attackDone;

    private void Start()
    { 
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        currentHealth = maxHealth;
        state = State.None;
        nextState = State.Idle;
        SetNewPatrolTarget();
    }

    private void Update()
    {
        // 죽었으면 아무것도 하지 않기
        if (isDead)
            return;

        //1. 스테이트 전환 상황 판단
        if (nextState == State.None) 
        {
            switch (state) 
            {
                case State.Idle:
                    // 플레이어가 추적 범위 안에 있으면 Chase로 전환
                    if (player != null && Vector3.Distance(transform.position, player.position) < chaseRange)
                    {
                        nextState = State.Chase;
                    }
                    // 패트롤 목표에 도달하면 새로운 목표 설정
                    else if (navMeshAgent.remainingDistance < 0.5f)
                    {
                        SetNewPatrolTarget();
                    }
                    break;
                    
                case State.Chase:
                    // 플레이어가 추적 범위를 벗어나면 Idle로
                    if (player == null || Vector3.Distance(transform.position, player.position) > chaseRange + 2f)
                    {
                        nextState = State.Idle;
                        SetNewPatrolTarget();
                    }
                    // 공격 범위 안에 있으면 Attack
                    else if (Vector3.Distance(transform.position, player.position) < attackRange)
                    {
                        nextState = State.Attack;
                    }
                    break;
                    
                case State.Attack:
                    if (attackDone)
                    {
                        nextState = State.Chase; // 공격 후 다시 추적
                        attackDone = false;
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
                    animator.SetBool("isMoving", false);
                    break;
                case State.Chase:
                    animator.SetBool("isMoving", true);
                    break;
                case State.Attack:
                    animator.SetBool("isMoving", false);
                    Attack();
                    break;
                case State.Dead:
                    animator.SetBool("isMoving", false);
                    animator.SetTrigger("death"); // death 트리거 발동!
                    navMeshAgent.enabled = false;
                    isDead = true;
                    break;
            }
        }
        
        //3. 글로벌 & 스테이트 업데이트
        if (state == State.Idle)
        {
            navMeshAgent.SetDestination(patrolTarget);
        }
        else if (state == State.Chase && player != null)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
    
    private void SetNewPatrolTarget()
    {
        // 랜덤한 위치에서 패트롤 목표 설정
        Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
        randomDirection += transform.position;
        
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRange, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
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

    // 데미지 받는 함수
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // 데미지 받을 때 stun 애니메이션
        animator.SetTrigger("stun");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 죽는 함수
    private void Die()
    {
        if (isDead) return;

        isDead = true;
        
        Debug.Log("Enemy died!");
        
        // 즉시 사라지기
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // 공격 범위
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, attackRange);
        
        // 추적 범위
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, chaseRange);
    }
}