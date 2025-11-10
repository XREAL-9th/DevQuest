using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "ScriptableObjects/BulletData")]
public class BulletData : ScriptableObject
{
    [Header("Bullet Stats")]
    public float speed = 50f;
    public float lifetime = 3f;
    public float knockbackForce = 10f;

    [Header("Visual")]
    public GameObject hitEffectPrefab;
}
