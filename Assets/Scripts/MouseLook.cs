using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// camera look
public class MouseLook : MonoBehaviour
{
    [SerializeField]private float mouseSensitivity = 300f; //灵敏度
    public Transform playerBody;//玩家位置
    public float yRotation = 0f;




    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //Debug.Log("MouseX:" + mouseX);
        //Debug.Log("MouseY:" + mouseY);  输出mouseX，mouseY的值j
        yRotation -= mouseY;//将上下旋转的值进行累加
        yRotation = Mathf.Clamp(yRotation, -80f, 80f);
        playerBody.Rotate(Vector3.up * mouseX); //玩家横向旋转
        transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
    }
}
