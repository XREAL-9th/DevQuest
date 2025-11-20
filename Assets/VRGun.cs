using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class VRGun : MonoBehaviour
{
    [Header("XR")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    public Rigidbody gunRb;

    [Header("Shoot")]
    public Transform muzzle;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    public float recoilForce = 2f;

    [Header("Ammo")]
    public int maxAmmo = 10;
    public TextMeshProUGUI ammoText;

    XRBaseController controller;   // 햅틱 줄 컨트롤러
    Transform fireDirTransform;
    int currentAmmo;

    void Awake()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        grab.activated.AddListener(OnFire);          // 트리거 눌렀을 때
        grab.selectEntered.AddListener(OnSelect);    // 총 잡았을 때
        grab.selectExited.AddListener(OnDeselect);   // 놓았을 때
    }

    void OnDestroy()
    {
        grab.activated.RemoveListener(OnFire);
        grab.selectEntered.RemoveListener(OnSelect);
        grab.selectExited.RemoveListener(OnDeselect);
    }

    void OnSelect(SelectEnterEventArgs args)
    {
        controller = args.interactorObject.transform
                     .GetComponentInParent<XRBaseController>();
        // 🔥 발사 방향은 Select한 인터랙터(컨트롤러) 기준으로
        fireDirTransform = args.interactorObject.transform;
    }

    void OnDeselect(SelectExitEventArgs args)
    {
        controller = null;
        fireDirTransform = null;
    }

    void OnFire(ActivateEventArgs args)
    {
        if (currentAmmo <= 0) return;
        currentAmmo--;
        UpdateAmmoUI();

        // 🔥 발사 방향 = 총의 local X+ (오른쪽) 방향
        Vector3 dir = transform.right;

        // 총구에서 조금 앞에서 생성
        Vector3 spawnPos = muzzle.position + dir * 0.3f;

        GameObject b = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(dir));
        var rb = b.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * bulletForce;

        // 반동
        if (gunRb != null)
            gunRb.AddForce(-dir * recoilForce, ForceMode.Impulse);

        // 햅틱
        if (controller != null)
            controller.SendHapticImpulse(0.9f, 0.1f);
    }




    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo}/{maxAmmo}";
    }
}
