using UnityEngine;

public class HPBarCamera : MonoBehaviour
{
    private Camera cam;
    void Start() => cam = Camera.main;

    void LateUpdate()
    {
        if (cam)
            transform.forward = cam.transform.forward;
    }
}