using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("— Audio —")]
    public AudioSource audioSource; 
    public AudioClip shootClip;     

    [Header("— Shooting Settings —")]
    public float range = 100f;           
    public float damage = 1f;            
    public Camera fpsCam;                

    [Header("— Visual Effects —")]
    public GameObject hitEffectPrefab;   
    public float knockbackForce = 5f;    

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (audioSource != null && shootClip != null)
        {
            audioSource.PlayOneShot(shootClip, 0.8f); 
        }
        
        if (fpsCam == null)
        {
            Debug.LogWarning("FPS Camera not assigned!");
            return;
        }

        Ray ray = fpsCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log($"Hit object: {hit.transform.name}");

            if (hitEffectPrefab != null)
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));

            if (hit.transform.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.transform.GetComponentInParent<EnemyHealth>();
            
                Enemy enemyAI = hit.transform.GetComponentInParent<Enemy>();

                if (enemyHealth != null && !enemyHealth.IsDead)
                {
                    enemyHealth.TakeDamage((int)damage);
                    
                    Vector3 knockDir = (hit.transform.position - ray.origin).normalized;
                    enemyHealth.RequestKnockback(knockDir, knockbackForce); 
                }
                else
                {
                    Debug.Log("Hit ignored — enemy already dead or null.");
                }
            }
        }
    }
}