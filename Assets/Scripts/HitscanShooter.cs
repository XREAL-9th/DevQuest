using UnityEngine;
using UnityEngine.InputSystem;

public class HitscanShooter : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AimRayProvider aim;
    [Header("Settings")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask hitMask = ~0;
    [SerializeField] private int damage = 10;

    private int lastShotFrame = -1;

    void Reset()
    {
        aim = FindFirstObjectByType<AimRayProvider>();
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (lastShotFrame == Time.frameCount) return;
        lastShotFrame = Time.frameCount;

        if (aim == null) return;

        Ray ray = aim.GetRayFromCursor();
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitMask))
        {
            var hp = hit.collider.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
        }
    }
}