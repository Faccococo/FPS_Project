using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    public float amout;

    public float smoothAmout;

    public float maxAmout;

    [SerializeField]private Vector3 originPosition; // ≥ı ºŒª÷√


    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float movementX = -Input.GetAxis("Mouse X") * amout;
        float movementY = -Input.GetAxis("Mouse Y") * amout;


    Mathf.Clamp(movementX, -maxAmout, maxAmout);

        Vector3 finallyPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finallyPosition + originPosition, Time.deltaTime * smoothAmout);
    }
}
