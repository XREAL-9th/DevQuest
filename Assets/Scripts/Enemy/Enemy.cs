using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // ===== ENUM =====
    public enum State
    {
        None,
        Idle,
        Wander,
        Chase,
        RangedAttack,
        Dash,
        Dead
    }

    [Header("— Components —")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform player;
    public GameObject projectilePrefab;
    public Transform projectileSpawn;

    [Header("— Detection —")]
    public float detectRange = 10f;
    public float attackRange = 6f;
    public float dashRange = 3f;

    [Header("— Wander —")]
    public float wanderRadius = 8f;
    public float wanderDelay = 3f;

    [Header("— Dash —")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.4f;
    public float dashCooldown = 3f;

    [Header("— Ranged Attack —")]
    public float projectileSpeed = 12f;
    public float attackCooldown = 2f;

    [Header("— Health —")]
    public int maxHP = 5;
    private int currentHP;

    [Header("— Debug —")]
    public State state = State.None;
    public State nextState = State.None;
    public bool showGizmos = true;

    // 내부 제어 변수
    private bool isDead = false;
    private bool attackDone;
    private float wanderTimer;
    private float attackTimer;
    private float dashTimer;
    private float dashCDTimer;
    private bool dashing;

    void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (!projectileSpawn) projectileSpawn = transform;

        currentHP = maxHP;
        state = State.None;
        nextState = State.Idle;
        wanderTimer = wanderDelay;
    }

    void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime;
        dashCDTimer -= Time.deltaTime;

        // ===== 상태 전이 판단 =====
        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Idle:
                case State.Wander:
                    if (PlayerInRange(dashRange)) nextState = State.Dash;
                    else if (PlayerInRange(attackRange)) nextState = State.RangedAttack;
                    else if (PlayerInRange(detectRange)) nextState = State.Chase;
                    else if (state == State.Idle) nextState = State.Wander;
                    break;

                case State.Chase:
                    if (PlayerInRange(dashRange)) nextState = State.Dash;
                    else if (PlayerInRange(attackRange)) nextState = State.RangedAttack;
                    else if (!PlayerInRange(detectRange * 1.5f)) nextState = State.Wander;
                    else if (!PlayerInRange(detectRange * 2f)) nextState = State.Idle;
                    break;

                case State.RangedAttack:
                    if (attackDone) { nextState = State.Chase; attackDone = false; }
                    break;

                case State.Dash:
                    if (dashing && dashTimer <= 0f)
                        nextState = State.Chase;
                    break;
            }
        }

        // ===== 상태 전이 발생 =====
        if (nextState != State.None)
        {
            state = nextState;
            nextState = State.None;

            switch (state)
            {
                case State.Idle:
                    agent.isStopped = true;
                    PlayAnim("Idle");
                    break;

                case State.Wander:
                    agent.isStopped = false;
                    PlayAnim("Run");
                    break;

                case State.Chase:
                    agent.isStopped = false;
                    PlayAnim("Run");
                    break;

                case State.RangedAttack:
                    agent.isStopped = true;
                    StartCoroutine(CoRangedAttack());
                    break;

                case State.Dash:
                    StartCoroutine(CoDash());
                    break;

                case State.Dead:
                    Die(); // ✅ 사망 처리
                    break;
            }
        }

        // ===== 상태별 지속 로직 =====
        switch (state)
        {
            case State.Wander:
                Wander();
                break;

            case State.Chase:
                if (player)
                    agent.SetDestination(player.position);
                break;
        }
    }

    // ===== Wander =====
    private void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            Vector3 randomDir = Random.insideUnitSphere * wanderRadius + transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, 1))
                agent.SetDestination(hit.position);
            wanderTimer = wanderDelay;
        }
    }

    // ===== Dash =====
    private IEnumerator CoDash()
    {
        if (dashing || dashCDTimer > 0f) yield break;
        dashing = true;
        dashTimer = dashDuration;
        dashCDTimer = dashCooldown;

        PlayAnim("Dash");
        Vector3 dir = (player.position - transform.position).normalized;
        float timer = 0f;
        agent.isStopped = true;

        while (timer < dashDuration)
        {
            transform.position += dir * dashSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        dashing = false;
        agent.isStopped = false;
        nextState = State.Chase;
    }

    // ===== Ranged Attack =====
    private IEnumerator CoRangedAttack()
    {
        PlayAnim("Attack");
        yield return new WaitForSeconds(0.5f);

        if (attackTimer <= 0f && projectilePrefab)
        {
            attackTimer = attackCooldown;
            Vector3 dir = (player.position + Vector3.up * 0.9f - projectileSpawn.position).normalized;

            GameObject proj = Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.LookRotation(dir));
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = dir * projectileSpeed;
        }

        yield return new WaitForSeconds(0.8f);
        attackDone = true;
    }

    // ===== Death =====
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        agent.isStopped = true;
        StopAllCoroutines();
        PlayAnim("Die");
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            Material mat = rend.material;
            Color color;

            // URP 호환
            if (mat.HasProperty("_BaseColor"))
                color = mat.GetColor("_BaseColor");
            else if (mat.HasProperty("_Color"))
                color = mat.GetColor("_Color");
            else
                yield break;

            // 페이드 아웃
            for (float t = 1f; t >= 0; t -= Time.deltaTime)
            {
                color.a = t;
                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", color);
                else if (mat.HasProperty("_Color"))
                    mat.SetColor("_Color", color);
                yield return null;
            }
        }

        Destroy(gameObject);
    }

    // ===== Utility =====
    bool PlayerInRange(float range)
    {
        if (!player) return false;
        return Vector3.Distance(transform.position, player.position) <= range;
    }

    void PlayAnim(string name)
    {
        if (animator)
            animator.Play(name);
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return; // ✅ 이미 사망 상태 방지
        currentHP -= dmg;
        Debug.Log($"Enemy took {dmg} damage! Current HP: {currentHP}");
        if (currentHP <= 0)
            nextState = State.Dead;
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, wanderRadius);
        Gizmos.color = Color.blue; Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.magenta; Gizmos.DrawWireSphere(transform.position, dashRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
                Debug.Log("Player hit by Enemy!");
            }
        }
    }
}
