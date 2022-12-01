using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// camera look
public class MouseLook : MonoBehaviour
{
    [SerializeField]private float mouseSensitivity = 300f; //������
    public Transform playerBody;//���λ��
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
        //Debug.Log("MouseY:" + mouseY);  ���mouseX��mouseY��ֵj
        yRotation -= mouseY;//��������ת��ֵ�����ۼ�
        yRotation = Mathf.Clamp(yRotation, -80f, 80f);
        playerBody.Rotate(Vector3.up * mouseX); //��Һ�����ת
        transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
    }
}
