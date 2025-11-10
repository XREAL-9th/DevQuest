using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingControl : MonoBehaviour
{
    [Header("Preset Fields")]
    [SerializeField] private Camera mainCamera;
    
    [Header("Settings")]
    [SerializeField] private float rayDistance = 1000f;
    [SerializeField] private int damagePerShot = 1;
    
    private int enemyLayerMask;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = GetComponentInChildren<Camera>();
        
        enemyLayerMask = 1 << LayerMask.NameToLayer("Enemy");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Ray screenRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Ray ray = new Ray(mainCamera.transform.position, screenRay.direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, enemyLayerMask))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damagePerShot);
                Debug.Log("Hit enemy!");
            }
        }
    }
}