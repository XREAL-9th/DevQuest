using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Preset Fields")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject splashFx;
    [SerializeField] private NavMeshAgent agent;

    [Header("Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float sightRange = 8f;
    [SerializeField] private float fov = 120f;
    [SerializeField] private float wanderRadius = 6f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Health UI")]
    [SerializeField] private Canvas healthBarCanvas;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private float healthBarHeight = 2f;

    [Header("Death Effects")]
    [SerializeField] private GameObject deathEffect;

    private Vector3 wanderTarget;
    private float lastAttackTime;

    public enum State
    {
        None,
        Idle,
        Wander,
        Chase,
        Attack
    }

    [Header("Debug")]
    public State state = State.None;
    public State nextState = State.None;

    private bool attackDone;
    private Transform player;
    private bool isDead = false;
    private Camera mainCamera;

    private void Start()
    {
        state = State.None;
        nextState = State.Idle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;

        currentHealth = maxHealth;

        if (healthBarCanvas != null)
        {
            healthBarCanvas.gameObject.SetActive(true); // true로 변경!
        }
        UpdateHealthBar();

        PickNewWanderPoint();
    }

    private void Update()
    {
        if (isDead) return;

        // 체력바가 항상 카메라를 향하도록
        if (healthBarCanvas != null && healthBarCanvas.gameObject.activeSelf && mainCamera != null)
        {
            healthBarCanvas.transform.LookAt(healthBarCanvas.transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }

        //1. 상태 전환 조건
        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Idle:
                    if (PlayerInSight())
                    {
                        nextState = State.Chase;
                    }
                    else
                    {
                        nextState = State.Wander;
                    }
                    break;

                case State.Wander:
                    if (PlayerInSight())
                    {
                        nextState = State.Chase;
                    }
                    else
                    {
                        if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        {
                            PickNewWanderPoint();
                        }
                    }
                    break;

                case State.Chase:
                    if (Vector3.Distance(transform.position, player.position) <= attackRange
                        && Time.time - lastAttackTime > attackCooldown)
                    {
                        nextState = State.Attack;
                    }
                    else if (!PlayerInSight() && Vector3.Distance(transform.position, player.position) > sightRange * 1.5f)
                    {
                        nextState = State.Idle;
                    }
                    break;

                case State.Attack:
                    if (attackDone || Time.time - lastAttackTime > 2f)
                    {
                        attackDone = false;

                        if (Vector3.Distance(transform.position, player.position) <= attackRange * 1.5f)
                        {
                            nextState = State.Chase;
                        }
                        else if (PlayerInSight())
                        {
                            nextState = State.Chase;
                        }
                        else
                        {
                            nextState = State.Idle;
                        }
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
                    agent.isStopped = true;
                    agent.speed = wanderSpeed;
                    animator.SetBool("walk", false);
                    animator.SetBool("attack", false);
                    break;

                case State.Wander:
                    agent.isStopped = false;
                    agent.speed = wanderSpeed;
                    animator.SetBool("walk", true);
                    animator.SetBool("attack", false);
                    PickNewWanderPoint();
                    break;

                case State.Chase:
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    animator.SetBool("walk", true);
                    animator.SetBool("attack", false);
                    break;

                case State.Attack:
                    agent.isStopped = true;
                    lastAttackTime = Time.time;
                    animator.SetBool("walk", false);
                    animator.SetBool("attack", true);
                    Attack();
                    break;
            }
        }

        //3. 글로벌 업데이트
        if (state == State.Chase)
        {
            agent.SetDestination(player.position);

            Vector3 direction = (player.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
    }

    private void LateUpdate()
    {
        // 체력바 위치 업데이트
        if (healthBarCanvas != null && healthBarCanvas.gameObject.activeSelf)
        {
            Vector3 healthBarPosition = transform.position + Vector3.up * healthBarHeight;
            healthBarCanvas.transform.position = healthBarPosition;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} 체력: {currentHealth}/{maxHealth}");

        if (healthBarCanvas != null)
        {
            healthBarCanvas.gameObject.SetActive(true);
        }
        UpdateHealthBar();

        if (state == State.Idle || state == State.Wander)
        {
            nextState = State.Chase;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;

            if (currentHealth / maxHealth > 0.5f)
                healthBarFill.color = Color.green;
            else if (currentHealth / maxHealth > 0.2f)
                healthBarFill.color = Color.yellow;
            else
                healthBarFill.color = Color.red;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} 사망!");

        agent.isStopped = true;
        this.enabled = false;

        if (healthBarCanvas != null)
        {
            healthBarCanvas.gameObject.SetActive(false);
        }

        animator.SetTrigger("death");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 2f);
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
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(dirToPlayer.x, 0, dirToPlayer.z));

        animator.SetBool("attack", true);

        // 공격 애니메이션 타이밍에 맞춰 데미지 (0.3~0.7초 사이로 조정)
        Invoke("DealDamageToPlayer", 0.5f);
    }

    public void InstantiateFx()
    {
        Instantiate(splashFx, transform.position, Quaternion.identity);
        DealDamageToPlayer();
    }

    public void WhenAnimationDone()
    {
        attackDone = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, attackRange);

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, sightRange);
    }

    public void DealDamageToPlayer()
    {
        Debug.Log($"[Enemy] DealDamageToPlayer 호출됨!");

        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            Debug.Log($"[Enemy] 거리: {dist}, attackRange: {attackRange}");

            if (dist <= attackRange)
            {
                IDamageable damageable = player.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(10f);
                    Debug.Log("[Enemy] 데미지 전달 완료!");
                }
            }
        }
    }
}