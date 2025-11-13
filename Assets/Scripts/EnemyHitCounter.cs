using UnityEngine;

public class EnemyHitCounter : MonoBehaviour
{
    [SerializeField] int maxHits = 3;          // n번 맞으면 사망
    [SerializeField] int scorePerKill = 1;     // 킬당 점수
    [SerializeField] HealthBarWorld hpBar;     // (인스펙터 미연결 대비 자동 연결)

    int hitCount = 0;
    bool isDead = false;

    void Awake()
    {
        // ✅ 인스펙터에 안 넣어도 자동으로 찾게 (프리팹/인스턴스 누락 방지)
        if (!hpBar)
            hpBar = GetComponentInChildren<HealthBarWorld>(true);
    }

    void Start()
    {
        if (hpBar) hpBar.SetRatio(1f);
        Debug.Log($"[Enemy] Start: hpBar={(hpBar ? hpBar.name : "NULL")}, maxHits={maxHits}");
    }

    public void TakeHit(int amount = 1)
    {
        if (isDead) return;

        hitCount += amount;
        float ratio = Mathf.Clamp01((float)(maxHits - hitCount) / maxHits);
        Debug.Log($"[Enemy] TakeHit: +{amount}, hitCount={hitCount}/{maxHits}, ratio={ratio}");

        if (hpBar)
        {
            hpBar.SetRatio(ratio);
        }
        else
        {
            // ✅ 정말로 hpBar가 연결이 안 됐다면 여기서도 한 번 더 시도
            hpBar = GetComponentInChildren<HealthBarWorld>(true);
            if (hpBar)
            {
                Debug.Log("[Enemy] Rewired hpBar at runtime");
                hpBar.SetRatio(ratio);
            }
            else
            {
                Debug.LogWarning("[Enemy] hpBar is NULL (프리팹에 EnemyHPBar/HealthBarWorld 연결 누락)");
            }
        }

        if (hitCount >= maxHits)
            Die();
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("EnemyBullet"))
        {
            Destroy(other.gameObject);
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scorePerKill);
            GameManager.Instance.ReportEnemyDied();
        }

        if (hpBar) Destroy(hpBar.gameObject);
        Debug.Log("[Enemy] Die()");
        Destroy(gameObject);
    }
}
