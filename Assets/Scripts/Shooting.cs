using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Shooting Setting")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private GameObject hitEffect;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = GetComponent<Camera>();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameState != GameManager.GameState.Playing)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width /2, Screen.height /2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Health enemyHealth = hit.collider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Rigidbody enemyRb = hit.collider.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                enemyRb.AddForce(ray.direction * -1 * knockbackForce, ForceMode.Impulse);
            }

            if (hitEffect != null)
            {
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

        }
    }
}
