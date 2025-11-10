using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        None,
        Idle,
        Chase,
        RangedAttack
    }

    [Header("Preset Fields")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject splashFx;

    [Header("Detect/Combat")]
    [SerializeField] private float attackRange = 1.5f;  // 근접 공격 발동 거리
    [SerializeField] private float sightRange = 0f;    // 시야 범위(정면에서만 유효)
    [SerializeField] private float sightFOV = 90f;    // 정면 시야각(±45°)
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float idleSpeed = 1.5f;
    [SerializeField] private float stopDistance = 1.2f;  // 플레이어 접근 시 멈춤 거리

    [Header("Ranged Attack")]
    [SerializeField] private float rangedRange = 20f; // 원거리 공격 발동 거리
    [SerializeField] private float fireCoolTime = 1.6f; // 발사 쿨타임
    [SerializeField] private float fireWindUp = 0.2f; // 쏘기 전 딜레이
    [SerializeField] private float turnSpeed = 540f; // 초당 회전각
    [SerializeField] private GameObject projectilePrefab; // rigidbody
    [SerializeField] private Transform muzzle; // 발사 위치


    [Header("Wander (필수과제 2-1)")]
    [SerializeField] private float wanderRadius = 8f;
    [SerializeField] private float waypointTolerance = 0.4f;
    [SerializeField] private Vector2 wanderDelayRange = new Vector2(1f, 3f);

    [Header("Perception")]
    [SerializeField] private Transform eyePoint;     // 눈 위치(자식 오브젝트 할당)
    [SerializeField] private float fallbackEyeHeight = 1.0f; // eyePoint 없을 때만 사용

    [Header("Debug")]
    public State state = State.None;
    public State nextState = State.None;

    private bool attackDone;
    private float fireTimer; // 원기리 공격 쿨타임
    private float wanderDelayTimer;
    private NavMeshAgent agent;
    private Transform player;   // 가장 가까운 플레이어(레이어 6)

    private readonly int lmPlayer = 1 << 6;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;
        agent.updateRotation = true;
        agent.updateUpAxis = true;
        agent.autoBraking = false;
        agent.areaMask = NavMesh.AllAreas;
    }

    private void Start()
    {
        // 시작 위치가 NavMesh 바깥이면 가장 가까운 위치로 워프
        if (NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
            agent.Warp(hit.position);
        state = State.None;
        nextState = State.Idle;
    }

    private void Update()
    {
        // 전이 판단
        if (nextState == State.None)
        {
            switch (state)
            {
                case State.Idle:
                    if (PlayerInFront(out player))
                    {
                        float d = Vector3.Distance(transform.position, player.position);
                        nextState = (d <= attackRange) ? State.Chase : (d <= rangedRange && HasLineOfSight()) ? State.RangedAttack : State.Chase;
                    }
                    break;
                case State.Chase:
                    if (!PlayerInFront(out player)) nextState = State.Idle;
                    else
                    {
                        float d = Vector3.Distance(transform.position, player.position);
                        if (d <= attackRange)
                        {
                            if (attackDone) attackDone = false;
                        }
                        else if (d <= rangedRange && HasLineOfSight()) nextState = State.RangedAttack;
                    }
                    break;
                case State.RangedAttack:
                    if (!PlayerInFront(out player)) nextState = State.Idle;
                    else
                    {
                        float d = Vector3.Distance(transform.position, player.position);
                        if (d <= attackRange) nextState = State.Chase;    // 근접으로 전환
                        else if (d > rangedRange || !HasLineOfSight()) nextState = State.Chase;
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
                    agent.speed = idleSpeed;
                    agent.isStopped = false;
                    agent.stoppingDistance = 0f;   // ★ 배회할 땐 0으로
                    PickNextWander();
                    break;

                case State.Chase:
                    agent.speed = chaseSpeed;
                    agent.isStopped = false;
                    agent.stoppingDistance = stopDistance; // ★ 추적/공격 때만 사용
                    break;

                case State.RangedAttack:
                    agent.isStopped = true; // 멈추고 조준
                    fireTimer = Mathf.Max(0f, fireTimer); // 유지
                    break;
            }
        }
        switch (state)
        {
            case State.Idle:
                UpdateIdle();     // 랜덤 배회 이동
                break;

            case State.Chase:
                UpdateChase();   // 플레이어 추적 + 공격
                break;
            case State.RangedAttack:
                UpdateRanged();
                break;
        }

        //3. 글로벌 & 스테이트 업데이트
        //insert code here...
    }
    private void UpdateIdle()
    {
        if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            PickNextWander();
            return;
        }

        float arriveThresh = Mathf.Max(waypointTolerance, agent.stoppingDistance) + 0.05f;
        if (!agent.pathPending && agent.remainingDistance <= arriveThresh)
        {
            wanderDelayTimer -= Time.deltaTime;
            if (wanderDelayTimer <= 0f) PickNextWander();
        }
    }

    private void PickNextWander()
    {
        if (TryGetRandomPoint(transform.position, wanderRadius, out var p))
        {
            agent.isStopped = false;            // 혹시 멈춰있으면 해제
            agent.SetDestination(p);
        }
        wanderDelayTimer = Random.Range(wanderDelayRange.x, wanderDelayRange.y);
    }

    private bool TryGetRandomPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 15; i++)
        {
            // 평면(XZ)에서만 랜덤 → Y는 center.y 유지
            Vector2 r = UnityEngine.Random.insideUnitCircle * radius;
            Vector3 random = new Vector3(center.x + r.x, center.y, center.z + r.y);

            if (NavMesh.SamplePosition(random, out var hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = center;
        return false;
    }


    // ================== Attack(추적/공격) ==================
    private void UpdateChase()
    {
        if (player == null) return;

        // 추적
        agent.isStopped = false;
        agent.stoppingDistance = stopDistance;
        agent.SetDestination(player.position);

        // 도착하면 바라보게
        var to = (player.position - transform.position);
        to.y = 0f;
        if (to.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(to), turnSpeed * Time.deltaTime);

        // 근접 공격 범위면 멈추고 공격 트리거
        if (!agent.pathPending && agent.remainingDistance <= attackRange)
        {
            agent.isStopped = true;
            Attack();   // 애니메이션 이벤트로 attackDone=true 세팅
        }
    }

    private void UpdateRanged()
    {
        if (player == null) return;

        // 플레이어 바라보기 (제자리 조준)
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.0001f)
        {
            var look = Quaternion.LookRotation(dir.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f && HasLineOfSight() && Vector3.Distance(transform.position, player.position) <= rangedRange)
        {
            StartCoroutine(FireOnce());
            fireTimer = fireCoolTime;
        }
    }
    // ================== 감지/시야 체크 ==================
    private bool PlayerInFront(out Transform found)
    {
        found = null;

        // 근처 플레이어 찾기
        var cols = Physics.OverlapSphere(transform.position, sightRange, lmPlayer, QueryTriggerInteraction.Ignore);
        if (cols == null || cols.Length == 0) return false;

        // 가장 가까운 대상 선택
        float best = float.MaxValue;
        Transform bestT = null;
        foreach (var c in cols)
        {
            float d = (c.transform.position - transform.position).sqrMagnitude;
            if (d < best) { best = d; bestT = c.transform; }
        }

        if (bestT == null) return false;

        // 정면 각도 체크
        Vector3 dir = (bestT.position - transform.position);
        dir.y = 0f;
        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > sightFOV * 0.5f) return false;

        // 가려짐 체크(옵션)
        Vector3 origin = eyePoint ? eyePoint.position : (transform.position + Vector3.up * fallbackEyeHeight);
        Vector3 dirNorm = (bestT.position - origin).normalized;

        // 정면 각도 체크는 origin 기준으로 다시 계산(수직 성분 제거)
        Vector3 flatToTarget = bestT.position - origin; flatToTarget.y = 0f;
        if (Vector3.Angle(transform.forward, flatToTarget) > sightFOV * 0.5f) return false;

        // 가려짐 체크
        if (Physics.Raycast(origin, dirNorm, out var hit, sightRange))
        {
            if (((1 << hit.collider.gameObject.layer) & lmPlayer) == 0) return false;
        }


        found = bestT;
        return true;
    }
    private bool HasLineOfSight()
    {
        if (player == null) return false;
        Vector3 origin = eyePoint ? eyePoint.position : (transform.position + Vector3.up * fallbackEyeHeight);
        Vector3 target = player.position + Vector3.up * 1.1f;
        Vector3 dir = (target - origin);
        float dist = dir.magnitude;
        if (Physics.Raycast(origin, dir.normalized, out var hit, dist, ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.gameObject.layer == 6; // 플레이어 레이어
        }
        return true;
    }

    private IEnumerator FireOnce()
    {
        yield return new WaitForSeconds(fireWindUp);
        if (!projectilePrefab || !muzzle) yield break;

        // 총구 바로 앞에서 스폰 (자기 몸 밖으로 조금 빼주기)
        Vector3 spawnPos = muzzle.position + muzzle.forward * 0.25f;
        Quaternion spawnRot = muzzle.rotation;

        GameObject go = Instantiate(projectilePrefab, spawnPos, spawnRot);

        // 1) 스폰 직후 Enemy와의 충돌 무시
        var myCols = GetComponentsInChildren<Collider>();
        var projCols = go.GetComponentsInChildren<Collider>();
        foreach (var pc in projCols)
            foreach (var mc in myCols)
                Physics.IgnoreCollision(pc, mc, true);

        // 2) 속도 부여 (Unity 6 경고 대응)
        if (go.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = muzzle.forward * 12f;   // 보기 좋게 12~16으로 시작
        }
        else if (go.TryGetComponent(out Projectile p))
        {
            p.Launch(muzzle.forward * 12f);
        }

        animator?.SetTrigger("shoot");  // 애니메이터에 shoot 파라미터 없으면 제거
    }


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


    private void OnDrawGizmosSelected()
    {
        //Gizmos를 사용하여 공격 범위를 Scene View에서 확인할 수 있게 합니다. (인게임에서는 볼 수 없습니다.)
        //해당 함수는 없어도 기능 상의 문제는 없지만, 기능 체크 및 디버깅을 용이하게 합니다.
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
}
