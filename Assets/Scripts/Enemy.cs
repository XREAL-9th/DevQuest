using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;

    [Header("Settings")]
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float detectAngle = 60f;
    [SerializeField] private float wanderRadius = 8f;
    [SerializeField] private float wanderInterval = 4f;
    [SerializeField] private float wanderSpeed = 2.0f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 1.2f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3; // 피격 허용 횟수 (총알 3번 맞으면 사망)
    private int currentHealth;

    public enum State
    {
        None,
        Idle,
        Wander,
        Chase,
        Dash
    }

    [Header("Debug")]
    public State state = State.None;
    public State nextState = State.None;

    private Vector3 wanderCenter;
    private float wanderTimer;

    private void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        state = State.None;
        nextState = State.Idle;
        wanderCenter = transform.position;
        wanderTimer = wanderInterval;

        currentHealth = maxHealth; // 시작 체력 세팅
    }

    private void Update()
    {
        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Idle:
                    if (IsPlayerVisible())
                        nextState = State.Chase;
                    else
                        nextState = State.Wander;
                    break;

                case State.Wander:
                    if (IsPlayerVisible())
                        nextState = State.Chase;
                    break;

                case State.Chase:
                    if (!player) break;

                    float dist = Vector3.Distance(transform.position, player.position);

                    if (dist <= dashDistance)
                        nextState = State.Dash;
                    else if (!IsPlayerVisible())
                        nextState = State.Wander;
                    break;

                case State.Dash:
                    break;
            }
        }

        if (nextState != State.None)
        {
            state = nextState;
            nextState = State.None;

            switch (state)
            {
                case State.Idle:
                    EnterIdle();
                    break;
                case State.Wander:
                    EnterWander();
                    break;
                case State.Chase:
                    EnterChase();
                    break;
                case State.Dash:
                    EnterDash();
                    break;
            }
        }

        switch (state)
        {
            case State.Wander:
                UpdateWander();
                break;
            case State.Chase:
                UpdateChase();
                break;
            case State.Dash:
                break;
        }
    }

    // --------------------------
    // 상태별 함수 정의
    // --------------------------

    private void EnterIdle()
    {
        animator.ResetTrigger("walk");
        animator.ResetTrigger("attack");
        animator.ResetTrigger("dash");
        animator.SetTrigger("idle");

        agent.ResetPath();
    }

    private void EnterWander()
    {
        animator.ResetTrigger("idle");
        animator.ResetTrigger("attack");
        animator.ResetTrigger("dash");
        animator.SetTrigger("walk");

        agent.speed = wanderSpeed;
        MoveToRandomPoint();
    }

    private void UpdateWander()
    {
        wanderTimer -= Time.deltaTime;

        if (agent.remainingDistance < 0.5f || wanderTimer <= 0f)
        {
            MoveToRandomPoint();
            wanderTimer = wanderInterval;
        }
    }

    private void EnterChase()
    {
        animator.ResetTrigger("idle");
        animator.ResetTrigger("walk");
        animator.ResetTrigger("dash");
        animator.SetTrigger("attack");

        agent.speed = chaseSpeed;
    }

    private void UpdateChase()
    {
        if (player)
            agent.SetDestination(player.position);
    }

    private void EnterDash()
    {
        animator.ResetTrigger("idle");
        animator.ResetTrigger("walk");
        animator.ResetTrigger("attack");
        animator.SetTrigger("dash");

        if (player)
        {
            agent.speed = chaseSpeed * 2.5f;
            agent.SetDestination(player.position);
        }

        Invoke(nameof(EndDash), dashDuration);
    }

    private void EndDash()
    {
        if (IsPlayerVisible())
            nextState = State.Chase;
        else
            nextState = State.Wander;
    }

    // --------------------------
    // 보조 기능 함수
    // --------------------------

    private void MoveToRandomPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomPoint = wanderCenter + new Vector3(randomCircle.x, 0, randomCircle.y);

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    private bool IsPlayerVisible()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = player.position - transform.position;
        float distToPlayer = dirToPlayer.magnitude;

        if (distToPlayer > detectRange) return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > detectAngle / 2f) return false;

        if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer.normalized, out RaycastHit hit, detectRange))
        {
            if (hit.transform == player)
                return true;
        }

        return false;
    }

    // --------------------------
    // 체력 및 피격 처리
    // --------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet"))
        {
            TakeDamage(1); // 총알 한 발에 1의 피해
            Destroy(other.gameObject); // 총알 제거
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 사망 애니메이션이나 이펙트 추가 가능
        Destroy(gameObject);
    }
    public void InstantiateFx() { }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, wanderRadius);

        Gizmos.color = new Color(0f, 0f, 1f, 0.25f);
        Gizmos.DrawSphere(transform.position, detectRange);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
        Gizmos.DrawSphere(transform.position, dashDistance);
    }
}
