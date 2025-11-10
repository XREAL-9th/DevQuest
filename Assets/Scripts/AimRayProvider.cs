using UnityEngine;
using UnityEngine.InputSystem;

public class AimRayProvider : MonoBehaviour
{
    Camera cam;

    void Awake() => cam = Camera.main;

    public Ray GetRayFromCursor()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        return cam.ScreenPointToRay(screenPos);
    }
}