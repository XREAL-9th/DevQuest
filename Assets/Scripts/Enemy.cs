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
    [SerializeField] private NavMeshAgent nmAgent;
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float attackRange;
    [SerializeField] private float detectionRange;
    [SerializeField] private float fieldOfView;
    [SerializeField] private float wanderRadius = 30f; 
    [SerializeField] private float minWanderWaitTime = 2f;
    [SerializeField] private float maxWanderWaitTime = 5f;

    private float wanderWaitTimer = -1f;
    private EnemySpawner mySpawner;

    public enum State
    {
        None,
        Wander, 
        Chase, 
        Attack, 
        Die
    }

    [Header("Debug")]
    public State state = State.None;
    public State nextState = State.None;

    private bool attackDone;

    private void Start()
    {
        state = State.None;
        nextState = State.Wander;

        nmAgent = GetComponent<NavMeshAgent>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    private void Update()
    {
        //1. 스테이트 전환 상황 판단
        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Wander:
                    if (CanSeeTarget())
                    {
                        nextState = State.Chase;
                    }
                    break;
                case State.Chase:
                    float distance = Vector3.Distance(transform.position, target.position);
                    if (distance <= attackRange)
                    {
                        nextState = State.Attack;
                    }
                    else if (!CanSeeTarget())
                    {
                        nextState = State.Wander;
                    }
                    break;
                case State.Attack:
                    if (attackDone)
                    {
                        nextState = State.Wander;
                        attackDone = false;
                    }
                    break;
                case State.Die:
                    Die();
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
                case State.Wander:
                    break;
                case State.Chase:
                    if (target != null)
                        nmAgent.SetDestination(target.position);
                    break;
                case State.Attack:
                    if(target != null)
                        transform.LookAt(target.position);
                    Attack();
                    break;

            }
        }

        //3. 글로벌 & 스테이트 업데이트
        switch (state)
        {
            case State.Wander:
                if (wanderWaitTimer > 0)
                {
                    wanderWaitTimer -= Time.deltaTime;
                    if (wanderWaitTimer <= 0)
                    {
                        GoToRandomPoint();
                    }
                }
                else
                {
                    if (!nmAgent.pathPending && (nmAgent.remainingDistance < 0.5f || !nmAgent.hasPath))
                    {
                        nmAgent.ResetPath();
                        wanderWaitTimer = Random.Range(minWanderWaitTime, maxWanderWaitTime);
                    }
                }
                break;
            case State.Chase:
                break;
            case State.Attack:
                break;

        }
    }

    private bool CanSeeTarget()
    {
        if (target == null)
            return false;

        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > detectionRange)
            return false;

        float angle = Vector3.Angle(directionToTarget, transform.forward);
        if (angle > fieldOfView / 2)
            return false;

        return true;
    }

    private void GoToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, NavMesh.AllAreas))
        {
            nmAgent.SetDestination(navHit.position);
            wanderWaitTimer = -1f; 
        }
    }

    /*
    private void GoNextWaypoint()
    {
        if ((waypoints.Length == 0))
        {
            return;
        }
        nmAgent.SetDestination(waypoints[currentWaypointIndex].position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }
    */
    private void Attack() //현재 공격은 애니메이션만 작동합니다.
    {
        animator.SetTrigger("attack");
    }

    public void InstantiateFx() //Unity Animation Event 에서 실행됩니다.
    {
        Instantiate(splashFx, transform.position, Quaternion.identity);
    }

    public void WhenAnimationDone() //Unity Animation Event 에서 실행됩니다.
    {
        attackDone = true;
    }

    public void Die()
    {
        if(mySpawner != null)
        {
            mySpawner.NotifyEnemyDeath();
        }
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        //Gizmos를 사용하여 공격 범위를 Scene View에서 확인할 수 있게 합니다. (인게임에서는 볼 수 없습니다.)
        //해당 함수는 없어도 기능 상의 문제는 없지만, 기능 체크 및 디버깅을 용이하게 합니다.
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, attackRange);

    }
}
