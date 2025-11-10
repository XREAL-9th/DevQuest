using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int damageToPlayer = 1;
    [SerializeField] private float attackCooldown = 2f;
    
    private PlayerHealth playerHealth;
    private Animator animator;
    private float lastAttackTime = 0f;

    private void Start()
    {
        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
        
        // Animator 찾기
        animator = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        // 플레이어와 충돌 중이고, 플레이어가 죽지 않았고, 공격 쿨타임이 지났으면 공격
        if (other.CompareTag("Player") && playerHealth != null && !playerHealth.IsDead())
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack(other.gameObject);
                lastAttackTime = Time.time;
            }
        }
    }

    private void Attack(GameObject target)
    {
        // 공격 애니메이션 재생
        if (animator != null)
        {
            animator.SetTrigger("attack");
        }
        
        playerHealth.TakeDamage(damageToPlayer);
        Debug.Log("Enemy attacked player!");
    }
}