using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerAim : MonoBehaviour
{
    public Camera cam;
    public GameObject gun;
    public Transform crosshairPoint;

    [Header("Ray Settings")]
    public float maxDistance = 100f;
    public float surfaceOffset = 0.02f;
    public string[] ignoreTags = { "Player", "CrossHairPoint" }; // ✅ 무시할 태그 목록

    // 🔹 PlayerFire와 공유할 상태값
    public bool HasAimHit { get; private set; } = false;
    public Vector3 AimHitPoint { get; private set; } = Vector3.zero;

    private PlayerInput playerInput;
    private InputAction aimAction;
    private bool isAiming = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        aimAction = playerInput.actions["Aim"];
        aimAction.performed += OnAimToggle;
    }

    void OnEnable() => aimAction.Enable();
    void OnDisable() => aimAction.Disable();

    private void OnAimToggle(InputAction.CallbackContext ctx)
    {
        isAiming = !isAiming;
        crosshairPoint.gameObject.SetActive(isAiming);
        if (gun != null) gun.SetActive(isAiming);

        Debug.Log(isAiming ? "🔍 [AIM] Aiming mode ON" : "❌ [AIM] Aiming mode OFF");
    }

    void Update()
    {
        if (!isAiming)
        {
            HasAimHit = false; // 에임 안할 땐 false
            return;
        }

        UpdateCrosshair();
    }

    private void UpdateCrosshair()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        // 첫 번째 Raycast
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            // 🎯 1️⃣ 맞은 물체가 무시할 Tag인지 검사
            if (ShouldIgnoreTag(hit.collider.tag))
            {
                Debug.Log($"⚠️ [AIM] Ignored tag: {hit.collider.tag}");

                // 🚀 2️⃣ 무시할 경우 → 그 뒤쪽까지 다시 Raycast
                if (Physics.Raycast(hit.point + ray.direction * 0.01f, ray.direction, out RaycastHit hit2, maxDistance))
                {
                    SetCrosshairAtHit(hit2);
                }
                else
                {
                    HideCrosshair(ray, "No valid hit after ignore");
                }
            }
            else
            {
                // ✅ 3️⃣ 일반적인 경우
                SetCrosshairAtHit(hit);
            }
        }
        else
        {
            // 🚫 아무것도 안 맞았을 때
            HideCrosshair(ray, "No hit at all");
        }
    }

    private bool ShouldIgnoreTag(string tag)
    {
        foreach (string t in ignoreTags)
        {
            if (tag == t) return true;
        }
        return false;
    }

    private void SetCrosshairAtHit(RaycastHit hit)
    {
        crosshairPoint.position = hit.point + hit.normal * surfaceOffset;
        crosshairPoint.rotation = Quaternion.LookRotation(hit.normal);

        if (!crosshairPoint.gameObject.activeSelf)
            crosshairPoint.gameObject.SetActive(true);

        // 🔹 Raycast 히트 상태 업데이트
        HasAimHit = true;
        AimHitPoint = hit.point;

        Debug.DrawLine(cam.transform.position, hit.point, Color.green);
        Debug.Log($"✅ [AIM] Hit '{hit.collider.name}' at {hit.point:F2}");
    }

    private void HideCrosshair(Ray ray, string reason)
    {
        if (crosshairPoint.gameObject.activeSelf)
            crosshairPoint.gameObject.SetActive(false);

        // 🔹 Raycast 미히트 상태로 갱신
        HasAimHit = false;
        AimHitPoint = ray.origin + ray.direction * maxDistance;

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * maxDistance, Color.red);
        Debug.Log($"⚪ [AIM] Crosshair hidden ({reason})");
    }
}
