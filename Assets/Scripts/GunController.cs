using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class GunController : MonoBehaviour
{
    [Header("총알 발사")]
    public Transform muzzlePoint; // 총구 위치 
    public GameObject bulletPrefab; // 총알 프리팹 

    [Header("햅틱 설정")]
    public float HapticIntensity = 0.7f; // 진동 강도
    public float HapticDuration = 0.15f; // 진동 시간

    [Header("남은 탄약 표시")]
    public TextMeshProUGUI remainAmmunition;
    private int currentAmmunition = 10;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        currentAmmunition = int.Parse(remainAmmunition.text);
    }

   public void ShootAndHaptics(ActivateEventArgs args)
    {
        // 총알 발사
        if (bulletPrefab != null && muzzlePoint != null && currentAmmunition > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

            // 발사 방향 설정
            Vector3 dir;
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f))
            {
                dir = (hit.point - bullet.transform.position).normalized;
            }
            else
            {
                dir = Camera.main.transform.forward;
            }

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = Vector3.zero;
            bulletRb.angularVelocity = Vector3.zero;
            bulletRb.AddForce(dir*50, ForceMode.Impulse);

            //GetComponent<AudioSource>().Play();

            // 남은 탄약 감소 및 UI 업데이트
            currentAmmunition = Mathf.Max(0, currentAmmunition - 1);
            remainAmmunition.text = currentAmmunition.ToString();
            Debug.Log("남은 탄약: " + currentAmmunition);
            
        }

        // 햅틱 피드백
        if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor)
        {
            XRBaseController controller = controllerInteractor.xrController;
            
            if (controller != null)
            {
                // 진동 명령 전송
                controller.SendHapticImpulse(HapticIntensity, HapticDuration);
            }
        }
    }
}
