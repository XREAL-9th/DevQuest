using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField][Range(1f, 20f)] private float sensitivity = 10f;
    private float mouseX, mouseY;
    private Transform playerTransform;

    private void Start()
    {
        //원활한 Debugging을 위해 마우스 커서를 보이지 않도록 하였습니다, Play 중 Esc 키를 누르면 마우스를 볼 수 있습니다.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerTransform = transform.parent;
    }

//Update(): 매 프레임마다 호출 (입력 처리에 적합)
//FixedUpdate(): 물리 업데이트마다 호출 (카메라 회전에는 부적합)
    private void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        playerTransform.rotation = Quaternion.Euler(new Vector3(0, mouseX, 0));

        mouseY += Input.GetAxis("Mouse Y") * sensitivity;
        mouseY = Mathf.Clamp(mouseY, -75f, 75f);
        transform.localRotation = Quaternion.Euler(new Vector3(-mouseY, 0, 0));
    }
}