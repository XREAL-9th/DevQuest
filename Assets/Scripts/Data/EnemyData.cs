using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 2)]
public class EnemyData : ScriptableObject
{
    [Header("Physical Attributes")]
    public float mass = 3f;                 
    public float knockbackResistance = 1f;  
    public PhysicsMaterial physicMaterial;   

    [Header("Gameplay Attributes")]
    public float health = 100f;             
    public float moveSpeed = 2f;        
}
