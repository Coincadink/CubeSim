using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{ 
    public float xSensitivity = 180.0f;
    public float ySensitivity = 5.0f;

    public float zoom = 6.0f;
    public float zoomRange = 3.0f;
    public float scrollSensitivity = 0.1f;

    public float smoothingFactor = 0.5f;

    private Vector3 xOffset;
    private float yOffset;
    private bool rotating;

    private float cameraDistance;

    void Start() 
    {
        cameraDistance = zoom;

        xOffset = new Vector3(0, 0, cameraDistance);
        yOffset = 0.0f;
    }

    void Update()
    {
        // While middle click is held, allow rotation.
        if (Input.GetMouseButton(2)) rotating = true;
        else rotating = false;

        if (rotating)
        {
            // Performs quaternion math to rotate in a circle around the Y axis.
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime, Vector3.up);
            xOffset = camTurnAngle * xOffset;

            // Allows the camera to be tilted up and down since quaternion math breaks at up and down.
            yOffset -= Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            yOffset = Mathf.Clamp(yOffset, -5.0f, 5.0f);
        }

        // Moved outside the rotating so Slerp doesn't end on release.
        transform.position = Vector3.Slerp(transform.position, xOffset + new Vector3(0, yOffset, 0), smoothingFactor);
        transform.LookAt(Vector3.zero);

        // Scales the camera's distance from the origin relative to scroll level.
        cameraDistance += -Input.mouseScrollDelta.y * scrollSensitivity;
        cameraDistance = Mathf.Clamp(cameraDistance, zoom - zoomRange, zoom + zoomRange);
        transform.position = Vector3.Slerp(transform.position, transform.position * (cameraDistance / transform.position.magnitude), smoothingFactor);
    }
}
