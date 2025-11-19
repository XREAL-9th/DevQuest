using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EnemyHealth))]
public class Enemy : MonoBehaviour
{
    public enum State { Idle, Walk, Run, Attack, Dash, Die }
    public State state = State.Idle;

    [Header("— Components —")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform player;
    public Rigidbody rb;
    public GameObject projectilePrefab;
    public Transform projectileSpawn;

    private EnemyHealth enemyHealth;

    [Header("— Audio —")]
    public AudioSource audioSource;
    public AudioClip enemyAttackClip;

    [Header("— Detection —")]
    public float detectRange = 12f;
    public float loseRange = 15f;
    public float attackRange = 6f;
    public float dashRange = 3f;

    [Header("— Wander —")]
    public float wanderRadius = 8f;
    public float wanderDelay = 3f;

    [Header("— Attack / Dash —")]
    public float projectileSpeed = 12f;
    public float attackCooldown = 1.5f;
    public float dashCooldown = 3f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.35f;

    [Header("— Animator Params —")]
    public string P_WANDER = "isWander";
    public string P_RUN = "isRun";
    public string P_ATTACK = "isAttack";
    public string P_DASH = "isDash";
    public string P_DEAD = "isDead";

    [Header("— Debug —")]
    public bool showGizmos = true;

    bool isDead => enemyHealth.IsDead;

    float wanderTimer;
    float attackTimer;
    float dashCDTimer;

    bool animatorReady;

    // ----------------------------------------------------------
    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        if (!animator)
            animator = GetComponentInChildren<Animator>();

        if (!audioSource)
            audioSource = GetComponent<AudioSource>();

        animatorReady = animator && animator.runtimeAnimatorController != null;
    }

    // ----------------------------------------------------------
    void Start()
    {
        enemyHealth.OnDeath += Die;
        enemyHealth.OnKnockbackApplied += ApplyKnockback;

        wanderTimer = wanderDelay;

        if (PlayerHealth.Instance)
            player = PlayerHealth.Instance.transform;
        else
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found) player = found.transform;
        }

        StartCoroutine(WaitAnimator());
    }

    IEnumerator WaitAnimator()
    {
        yield return null;
        GoIdle();
    }

    // ----------------------------------------------------------
    void Update()
    {
        if (!animatorReady || isDead) return;

        // 공통 안전 체크
        if (!agent || !agent.enabled || !agent.isOnNavMesh) return;

        attackTimer -= Time.deltaTime;
        dashCDTimer -= Time.deltaTime;

        float dist = player ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        switch (state)
        {
            case State.Idle:
            case State.Walk:
                if (dist <= dashRange && dashCDTimer <= 0f) GoDash();
                else if (dist <= attackRange && attackTimer <= 0f) GoAttack();
                else if (dist <= detectRange) GoRun();
                else DoWander();
                break;

            case State.Run:
                if (dist > loseRange) GoWander();
                else if (dist <= dashRange && dashCDTimer <= 0f) GoDash();
                else if (dist <= attackRange && attackTimer <= 0f) GoAttack();
                else ChasePlayer();
                break;
        }
    }

    // ----------------------------------------------------------
    // FSM
    // ----------------------------------------------------------
    void SafeBool(string p, bool b)
    {
        if (animator.HasParameterOfType(p, AnimatorControllerParameterType.Bool))
            animator.SetBool(p, b);
    }

    void SafeTrigger(string p)
    {
        if (animator.HasParameterOfType(p, AnimatorControllerParameterType.Trigger))
            animator.SetTrigger(p);
    }

    void GoIdle()
    {
        state = State.Idle;
        SafeBool(P_WANDER, false);
        SafeBool(P_RUN, false);
        if (agent && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = true;
    }

    public void GoWander()
    {
        if (!agent || !agent.enabled || !agent.isOnNavMesh) return;

        state = State.Walk;
        SafeBool(P_WANDER, true);
        SafeBool(P_RUN, false);
        agent.isStopped = false;
        PickWanderPoint();
    }

    void GoRun()
    {
        if (!agent || !agent.enabled || !agent.isOnNavMesh) return;

        state = State.Run;
        SafeBool(P_WANDER, false);
        SafeBool(P_RUN, true);
        agent.isStopped = false;
        ChasePlayer();
    }

    void GoAttack()
    {
        if (!agent || !agent.enabled || !agent.isOnNavMesh) return;

        state = State.Attack;
        agent.isStopped = true;

        if (player)
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        SafeTrigger(P_ATTACK);
        StartCoroutine(CoRangedAttack());
    }

    void GoDash()
    {
        if (!agent || !agent.enabled || !agent.isOnNavMesh) return;

        state = State.Dash;

        agent.isStopped = true;
        agent.enabled = false;

        SafeTrigger(P_DASH);
        StartCoroutine(CoDash());
    }

    // ----------------------------------------------------------
    // Wander
    // ----------------------------------------------------------
    void PickWanderPoint()
    {
        if (!agent || !agent.enabled || !agent.isOnNavMesh) return;

        Vector3 random = Random.insideUnitSphere * wanderRadius + transform.position;

        if (NavMesh.SamplePosition(random, out var hit, wanderRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    void DoWander()
    {
        if (!agent || !agent.enabled || !agent.isOnNavMesh) return;

        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f || Vector3.Distance(transform.position, agent.destination) < 1f)
        {
            PickWanderPoint();
            wanderTimer = wanderDelay;
        }
    }

    // ----------------------------------------------------------
    // Chase
    // ----------------------------------------------------------
    void ChasePlayer()
    {
        if (!player) return;
        if (!agent || !agent.enabled || !agent.isOnNavMesh) return;

        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(player.position, path))
            agent.SetPath(path);
    }

    // ----------------------------------------------------------
    // Attack
    // ----------------------------------------------------------
    IEnumerator CoRangedAttack()
    {
        yield return new WaitForSeconds(0.2f);

        if (projectilePrefab && projectileSpawn)
        {
            attackTimer = attackCooldown;

            GameObject proj = Instantiate(
                projectilePrefab, 
                projectileSpawn.position, 
                projectileSpawn.rotation
            );

            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
                ep.Fire(projectileSpawn.forward);

            if (audioSource && enemyAttackClip)
                audioSource.PlayOneShot(enemyAttackClip, 0.6f);
        }

        yield return new WaitForSeconds(0.4f);
        if (!isDead) GoRun();
    }

    // ----------------------------------------------------------
    // Dash
    // ----------------------------------------------------------
    IEnumerator CoDash()
    {
        dashCDTimer = dashCooldown;

        if (player)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            float t = 0f;

            while (t < dashDuration)
            {
                transform.position += dir * dashSpeed * Time.deltaTime;
                t += Time.deltaTime;
                yield return null;
            }
        }

        // NavMesh 복귀
        if (agent)
        {
            if (NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.enabled = true;

                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                }
            }
        }

        if (!isDead && agent.enabled) GoRun();
    }

    // ----------------------------------------------------------
    // Knockback
    // ----------------------------------------------------------
    public void ApplyKnockback(Vector3 dir, float force)
    {
        StartCoroutine(CoKnockback(dir, force));
    }

    IEnumerator CoKnockback(Vector3 direction, float force)
    {
        // NavMeshAgent 끄기
        if (agent)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }
        }

        // Rigidbody 밀기
        if (rb)
        {
            rb.isKinematic = false;
            rb.AddForce(direction.normalized * force, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(0.25f);

        if (rb) rb.isKinematic = true;

        // NavMeshAgent 복구
        if (agent)
        {
            if (NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.enabled = true;
            }

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                if (!isDead) GoRun();
            }
        }
    }

    // ----------------------------------------------------------
    // Death
    // ----------------------------------------------------------
    void Die()
    {
        if (state == State.Die) return;

        state = State.Die;

        StopAllCoroutines();

        // NavMeshAgent 안전 체크 추가
        if (agent)
        {
            if (agent.enabled && agent.isOnNavMesh)
                agent.isStopped = true;

            agent.enabled = false;
        }

        SafeTrigger(P_DEAD);

        GameManager.Instance.EnemyDefeated();

        StartCoroutine(FadeOutAndDestroy());
    }


    IEnumerator FadeOutAndDestroy()
    {
        yield return new WaitForSeconds(0.3f);

        Renderer rend = GetComponentInChildren<Renderer>();
        if (!rend)
        {
            Destroy(gameObject);
            yield break;
        }

        Material mat = rend.material;
        Color c = mat.color;

        for (float a = 1f; a >= 0; a -= Time.deltaTime * 2f)
        {
            c.a = a;
            mat.color = c;
            yield return null;
        }

        Destroy(gameObject);
    }

    // ----------------------------------------------------------
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, dashRange);
    }
}

// ----------------------------------------------------------
public static class AnimatorExtensions
{
    public static bool HasParameterOfType(this Animator ani, string name, AnimatorControllerParameterType type)
    {
        return ani.parameters.Any(p => p.name == name && p.type == type);
    }
}
