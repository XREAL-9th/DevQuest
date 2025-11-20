using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;

    [Header("Ammo Settings")]
    public int currentAmmo = 30;
    public int maxAmmo = 30;
    public int reserveAmmo = 120;

    [Header("UI Settings")]
    public TextMeshProUGUI ammoText;

    [Header("Haptic Settings")]
    public float hapticIntensity = 0.5f;
    public float hapticDuration = 0.1f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource fireSound;

    private XRGrabInteractable grabInteractable;
    private ActionBasedController controller;
    private bool canFire = true;
    private float fireRate = 0.2f;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // 이벤트 등록
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        UpdateAmmoUI();
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // ActionBasedController 가져오기
        var interactor = args.interactorObject;
        controller = interactor.transform.GetComponent<ActionBasedController>();

        if (controller != null)
        {
            Debug.Log("총을 잡았습니다!");

            // Activate 액션 등록 (Trigger 버튼)
            if (controller.activateAction != null && controller.activateAction.action != null)
            {
                controller.activateAction.action.performed += OnTriggerPressed;
            }
        }
        else
        {
            Debug.LogWarning("ActionBasedController를 찾을 수 없습니다!");
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (controller != null)
        {
            // 이벤트 해제
            if (controller.activateAction != null && controller.activateAction.action != null)
            {
                controller.activateAction.action.performed -= OnTriggerPressed;
            }

            controller = null;
            Debug.Log("총을 놓았습니다!");
        }
    }

    void Update()
    {
        // Grip 버튼으로 재장전 (Update에서 직접 체크)
        if (controller != null)
        {
            // Select Action 값 읽기 (0~1 값)
            float selectValue = 0f;
            if (controller.selectAction != null && controller.selectAction.action != null)
            {
                selectValue = controller.selectAction.action.ReadValue<float>();
            }

            // Grip 버튼이 눌렸을 때
            if (selectValue > 0.5f)
            {
                // 한 번만 실행되도록
                if (canFire) // canFire를 재사용
                {
                    Reload();
                    canFire = false;
                    Invoke("ResetFire", 0.5f);
                }
            }
        }
    }

    void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (canFire && currentAmmo > 0)
        {
            Fire();
        }
        else if (currentAmmo <= 0)
        {
            Debug.Log("탄약이 없습니다! 재장전이 필요합니다.");
        }
    }

    void Fire()
    {
        // 탄약 소모
        currentAmmo--;
        UpdateAmmoUI();

        Debug.Log($"발사! 남은 탄약: {currentAmmo}");

        // 총알 발사
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            }
            Destroy(bullet, 3f);
        }

        // 햅틱 피드백
        SendHapticFeedback(hapticIntensity, hapticDuration);

        // 이펙트
        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (fireSound != null)
            fireSound.Play();

        // 연사 제한
        canFire = false;
        Invoke("ResetFire", fireRate);
    }

    void Reload()
    {
        if (currentAmmo >= maxAmmo)
        {
            Debug.Log("탄창이 가득 찼습니다!");
            return;
        }

        if (reserveAmmo <= 0)
        {
            Debug.Log("보유 탄약이 없습니다!");
            return;
        }

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        UpdateAmmoUI();
        SendHapticFeedback(0.3f, 0.2f);

        Debug.Log($"재장전 완료! 탄약: {currentAmmo}/{reserveAmmo}");
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {reserveAmmo}";

            // 탄약 부족 시 색상 변경
            if (currentAmmo == 0)
            {
                ammoText.color = Color.red;
            }
            else if (currentAmmo <= 5)
            {
                ammoText.color = new Color(1f, 0.5f, 0f); // 주황색
            }
            else if (currentAmmo <= 10)
            {
                ammoText.color = Color.yellow;
            }
            else
            {
                ammoText.color = Color.white;
            }
        }
    }

    void SendHapticFeedback(float intensity, float duration)
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(intensity, duration);
        }
    }

    void ResetFire()
    {
        canFire = true;
    }

    void OnDestroy()
    {
        // 메모리 누수 방지
        if (controller != null && controller.activateAction != null && controller.activateAction.action != null)
        {
            controller.activateAction.action.performed -= OnTriggerPressed;
        }
    }
}