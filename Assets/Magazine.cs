using UnityEngine;

public class Magazine : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 30;      // 탄창 하나당 탄 수
    public int currentAmmo;

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }

    // 총에서 탄을 하나 쓸 때 호출
    public bool ConsumeOne()
    {
        if (currentAmmo <= 0)
            return false;

        currentAmmo--;
        return true;
    }

    public bool IsEmpty()
    {
        return currentAmmo <= 0;
    }
}
