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

    [Header("Ammo UI")]
    public TextMeshProUGUI ammoText;

    [Header("Magazine")]
    // 탄창이 꽂힐 XR 소켓
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor magazineSocket;
    // 현재 소켓에 꽂혀있는 탄창
    public Magazine currentMagazine;

    XRBaseController controller;   // 햅틱 줄 컨트롤러
    Transform fireDirTransform;    // 발사 방향 기준(컨트롤러 기준)

    void Awake()
    {
        UpdateAmmoUI();

        if (grab != null)
        {
            grab.activated.AddListener(OnFire);          // 트리거 눌렀을 때
            grab.selectEntered.AddListener(OnSelect);    // 총 잡았을 때
            grab.selectExited.AddListener(OnDeselect);   // 놓았을 때
        }

        // 탄창 소켓 이벤트 등록
        if (magazineSocket != null)
        {
            magazineSocket.selectEntered.AddListener(OnMagazineInserted);
            magazineSocket.selectExited.AddListener(OnMagazineRemoved);
        }
    }

    void OnDestroy()
    {
        if (grab != null)
        {
            grab.activated.RemoveListener(OnFire);
            grab.selectEntered.RemoveListener(OnSelect);
            grab.selectExited.RemoveListener(OnDeselect);
        }

        if (magazineSocket != null)
        {
            magazineSocket.selectEntered.RemoveListener(OnMagazineInserted);
            magazineSocket.selectExited.RemoveListener(OnMagazineRemoved);
        }
    }

    // 총을 잡았을 때
    void OnSelect(SelectEnterEventArgs args)
    {
        controller = args.interactorObject.transform.GetComponentInParent<XRBaseController>();

        // 발사 방향을 잡은 손(컨트롤러) 기준으로 할 때 사용
        fireDirTransform = args.interactorObject.transform;
    }

    // 총을 놓았을 때
    void OnDeselect(SelectExitEventArgs args)
    {
        controller = null;
        fireDirTransform = null;
    }

    // 탄창이 소켓에 꽂혔을 때
    void OnMagazineInserted(SelectEnterEventArgs args)
    {
        // 소켓에 들어온 오브젝트의 GameObject 가져오기
        var go = args.interactableObject.transform.gameObject;

        // 자식까지 포함해서 Magazine 찾기 (혹시 구조가 달라도 잡히도록)
        currentMagazine = go.GetComponentInChildren<Magazine>();

        if (currentMagazine == null)
        {
            Debug.LogWarning($"[VRGun] Magazine inserted but Magazine component not found on {go.name}");
        }
        else
        {
            Debug.Log($"[VRGun] Magazine inserted: {currentMagazine.currentAmmo}/{currentMagazine.maxAmmo}");
        }

        UpdateAmmoUI();
    }

    // 탄창이 소켓에서 빠졌을 때
    void OnMagazineRemoved(SelectExitEventArgs args)
    {
        var go = args.interactableObject.transform.gameObject;
        var mag = go.GetComponentInChildren<Magazine>();

        if (mag != null && mag == currentMagazine)
        {
            currentMagazine = null;
            Debug.Log("[VRGun] Magazine removed");
        }

        UpdateAmmoUI();
    }

    // 발사 트리거
    void OnFire(ActivateEventArgs args)
    {
        // 1) 탄창이 없으면 발사 불가
        if (currentMagazine == null)
        {
            // 빈방아쇠 느낌만 약하게
            if (controller != null)
                controller.SendHapticImpulse(0.3f, 0.05f);
            return;
        }

        // 2) 탄창에서 탄 하나 소모
        if (!currentMagazine.ConsumeOne())
        {
            // 탄창은 있지만 비어있음
            if (controller != null)
                controller.SendHapticImpulse(0.2f, 0.05f);
            UpdateAmmoUI();
            return;
        }

        // 3) 실제 발사
        UpdateAmmoUI();

        // 발사 방향: 컨트롤러 forward > 총구 forward 순으로 사용
        Vector3 dir;
        if (fireDirTransform != null)
            dir = fireDirTransform.forward;
        else if (muzzle != null)
            dir = muzzle.forward;
        else
            dir = transform.forward;

        // 총구 위치에서 생성
        Vector3 spawnPos = muzzle != null ? muzzle.position : transform.position;

        GameObject b = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(dir));
        var rb = b.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * bulletForce;   // velocity 사용

        // 반동
        if (gunRb != null)
            gunRb.AddForce(-dir * recoilForce, ForceMode.Impulse);

        // 햅틱 (강하게)
        if (controller != null)
            controller.SendHapticImpulse(0.9f, 0.1f);
    }

    void UpdateAmmoUI()
    {
        if (ammoText == null) return;

        if (currentMagazine == null)
        {
            ammoText.text = "0/0";
        }
        else
        {
            ammoText.text = $"{currentMagazine.currentAmmo}/{currentMagazine.maxAmmo}";
        }
    }
}
