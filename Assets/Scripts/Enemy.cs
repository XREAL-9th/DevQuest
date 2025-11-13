using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Preset Fields")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject splashFx;

    [Header("Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float detectRange = 20f;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private int attackDamage = 1;

    [SerializeField] private float attackCooldown = 1.0f; // ⭐ 공격 속도
    private float attackTimer = 0f;

    private NavMeshAgent agent;
    private Transform player;

    public enum State { None, Idle, Chase, Attack, Stun }
    public State state = State.None;
    public State nextState = State.None;

    private PlayerHealth playerHealth;

    private float stunTimer = 0f;
    private float stunDuration = 0.7f;


    private void Start()
    {
        nextState = State.Idle;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = moveSpeed;

        player = GameObject.FindWithTag("Player")?.transform;

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (player == null) return;

        // ⭐ 1) STUN 상태 우선 처리
        if (state == State.Stun)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
                nextState = State.Idle;
        }
        else
        {
            // ⭐ 2) 일반 FSM 전환
            if (nextState == State.None)
            {
                switch (state)
                {
                    case State.Idle:
                        if (IsPlayerInRange(detectRange))
                            nextState = State.Chase;
                        break;

                    case State.Chase:
                        if (IsPlayerInRange(attackRange))
                            nextState = State.Attack;
                        else if (!IsPlayerInRange(detectRange))
                            nextState = State.Idle;
                        break;

                    case State.Attack:
                        if (!IsPlayerInRange(attackRange))
                            nextState = State.Chase;
                        break;
                }
            }
        }

        // ⭐ 3) State Init
        if (nextState != State.None)
        {
            state = nextState;
            nextState = State.None;

            switch (state)
            {
                case State.Idle:
                    agent.isStopped = true;
                    animator.SetBool("isRunning", false);
                    break;

                case State.Chase:
                    agent.isStopped = false;
                    animator.SetBool("isRunning", true);
                    break;

                case State.Attack:
                    agent.isStopped = true;
                    animator.SetBool("isRunning", false);
                    animator.SetTrigger("attack");
                    attackTimer = 0f; // 공격 초기화
                    break;

                case State.Stun:
                    agent.isStopped = true;
                    animator.SetBool("isRunning", false);
                    animator.SetTrigger("stun");
                    stunTimer = stunDuration;
                    break;
            }
        }

        // ⭐ 4) 상태별 반복 행동

        if (state == State.Chase)
        {
            agent.SetDestination(player.position);
        }
        else if (state == State.Attack)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                TryDealDamage();
                attackTimer = attackCooldown;
            }
        }
    }


    private bool IsPlayerInRange(float range)
    {
        return Vector3.Distance(transform.position, player.position) <= range;
    }

    // ⭐ 애니메이션 이벤트 없어도 공격 들어감
    private void TryDealDamage()
    {
        if (playerHealth == null) return;

        if (IsPlayerInRange(attackRange + 0.5f))
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"🗡 Enemy hit Player → -{attackDamage} HP");
        }
    }

    // ⭐ Bullet에서 호출할 Stun
    public void ApplyStun()
    {
        if (state == State.Stun) return;
        nextState = State.Stun;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}