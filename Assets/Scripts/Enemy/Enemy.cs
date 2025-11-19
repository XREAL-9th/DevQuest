using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace XREAL
{
    public class Enemy : MonoBehaviour
    {
        [Header("Preset Fields")]
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject splashFx;

        [Header("Settings")]
        [SerializeField] private float attackRange;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField][Range(1f, 20f)] private float detectionRange = 10f;
        [SerializeField][Range(1f, 40f)] private float detectionOutRange = 30f;

        [Header("Health")]
        [SerializeField][Range(1f, 100f)] private float maxHp = 100f;

        [Header("Score")]
        [SerializeField][Range(1f, 100f)] private int score = 10;

        private float currentHp;
        public float CurrentHp
        {
            get => currentHp;
            set
            {
                currentHp = value;
                OnHpChanged?.Invoke(currentHp);
            }
        }

        public event Action<float> OnHpChanged;

        private AudioSource audioSource;
        private Vector3 destination;
        private GameObject targetPlayer;
        private Player player;

        public enum State
        {
            None,
            Idle,
            Wander,
            Target,
            Attack
        }

        [Header("Debug")]
        public State state = State.None;
        public State nextState = State.None;

        private bool attackDone;
        private bool detectPlayer;
        private bool isDead;

        void Awake()
        {
            TryGetComponent<AudioSource>(out audioSource);
        }

        private void Start()
        {
            state = State.None;
            nextState = State.Idle;

            CurrentHp = maxHp;
        }

        private void Update()
        {
            //1. 스테이트 전환 상황 판단
            if (nextState == State.None)
            {
                switch (state)
                {
                    case State.Idle:
                        //1 << 6인 이유는 Player의 Layer가 6이기 때문
                        if (Physics.CheckSphere(transform.position, attackRange, 1 << 6, QueryTriggerInteraction.Ignore) && targetPlayer != null)
                        {
                            nextState = State.Attack;
                            targetPlayer = null;
                        }
                        else
                        {
                            nextState = State.Wander;
                        }
                        break;
                    case State.Wander:
                        if ((this.transform.position.x == destination.x) && (this.transform.position.z == destination.z))
                        {
                            nextState = State.Idle;
                        }
                        if (detectPlayer == true)
                        {
                            nextState = State.Target;
                            detectPlayer = false;
                        }
                        break;
                    case State.Target:
                        if (targetPlayer == null)
                        {
                            nextState = State.Idle;
                        }
                        else
                        {
                            var dist = Vector3.Distance(transform.position, targetPlayer.transform.position);
                            if (dist > detectionOutRange)
                            {
                                nextState = State.Wander;
                                targetPlayer = null;
                            }
                            else if (dist <= attackRange && targetPlayer != null)
                            {
                                nextState = State.Attack;
                                targetPlayer = null;
                            }
                        }
                        break;
                    case State.Attack:
                        if (attackDone)
                        {
                            nextState = State.Idle;
                            attackDone = false;
                        }
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
                        break;
                    case State.Wander:
                        Walk();
                        break;
                    case State.Target:
                        Target();
                        break;
                    case State.Attack:
                        Attack();
                        break;
                        //insert code here...
                }
            }

            //3. 글로벌 & 스테이트 업데이트
            //insert code here...
            CheckFrontDetection();
        }

        private void Walk()
        {
            animator.SetTrigger("walk");
            Vector3 randomDirection = Random.insideUnitSphere * 5f;
            randomDirection.y = 0;
            destination = transform.position + randomDirection;
            navMeshAgent.SetDestination(destination);
        }

        private void Target()
        {
            animator.SetTrigger("walk");
            if (targetPlayer != null)
                navMeshAgent.SetDestination(targetPlayer.transform.position);
        }

        private void CheckFrontDetection()
        {
            if (state == State.Target && targetPlayer != null && currentHp > 0)
            {
                navMeshAgent.SetDestination(targetPlayer.transform.position);
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, detectionRange, 1 << 6, QueryTriggerInteraction.Ignore))
            {
                targetPlayer = hit.collider.gameObject;
                player = targetPlayer.GetComponent<Player>();
                detectPlayer = true;
            }
        }

        private void Attack() //현재 공격은 애니메이션만 작동합니다.
        {
            animator.SetTrigger("attack");

            if (player == null && targetPlayer != null)
            {
                player = targetPlayer.GetComponent<Player>();
            }

            player.TakeDamage(10f);
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

        public void TakeDamage(float damage)
        {
            if (isDead)
            {
                return;
            }

            audioSource.Play();

            CurrentHp -= damage;
            if (CurrentHp <= 0)
            {
                isDead = true;
                Die();
            }
        }

        private void Die()
        {
            Singleton.Game.AddScore(score);
            animator.SetTrigger("die");
            navMeshAgent.enabled = false;
            Destroy(gameObject, 1f);
        }
    }
}