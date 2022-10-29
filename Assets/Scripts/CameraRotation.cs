using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{   
    public float xSpeed = 180.0f;
    public float ySpeed = 5.0f;
    public float cameraDistance = 3.5f;
    public float smoothFactor = 0.5f;

    private Vector3 xOffset;
    private float yOffset;
    
    private bool rotating;

    void Start() 
    {
        xOffset = new Vector3(0, 0, -cameraDistance);
        yOffset = 0.0f;
    }

    void Update()
    {
        if (Input.GetMouseButton(2)) rotating = true;
        else rotating = false;

        if (rotating)
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime, Vector3.up);
            xOffset = camTurnAngle * xOffset;

            yOffset -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
            yOffset = Mathf.Clamp(yOffset, (float) (- cameraDistance / 1.5), (float) (cameraDistance / 1.5));

            transform.position = Vector3.Slerp(transform.position, xOffset + new Vector3(0, yOffset, 0), smoothFactor);
            transform.LookAt(Vector3.zero);
        }
    }
}
