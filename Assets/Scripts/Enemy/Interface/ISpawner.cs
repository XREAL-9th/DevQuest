using UnityEngine;

namespace XREAL.Interface
{
    public interface ISpawner
    {
        void Spawn(GameObject prefab, Vector3 position);
    }
}