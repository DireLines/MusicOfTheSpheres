using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //Rotation
    [SerializeField]
    float turnSpeed, vertSpeed, offsetDistance;
    [SerializeField]
    bool turnInverted, vertInverted;
    float turnDirection, vertDirection;

    //Zoom
    [SerializeField]
    float zoomMax, zoomMin, zoomSensitivity;
    float zoom, zoomSpeed;

    //Positioning
    [SerializeField]
    Transform target;
    public Transform Target { get => target; set => target = value; }

    [SerializeField]
    float lerpPercentage = 0.01f;

    private Vector3 targetPos;
    Vector3 velocityRef;
    private float yaw, pitch;


    void Start() {
        turnDirection = turnInverted ? -1 : 1;
        vertDirection = vertInverted ? -1 : 1;
        targetPos = target.position;

        yaw = 30f;
        pitch = 45f;
    }
    void LateUpdate() {
        //Rotation
        yaw += turnSpeed * turnDirection * Input.GetAxis("Camera Horizontal") * Time.deltaTime;
        pitch += vertSpeed * vertDirection * Input.GetAxis("Camera Vertical") * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, 5, 80);
        transform.eulerAngles = new Vector3(pitch, yaw);

        //Zoom
        zoom += Input.mouseScrollDelta.y * zoomSensitivity * 100f * Time.deltaTime;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);

        //Positioning
        targetPos = Vector3.SmoothDamp(targetPos, Target.position, ref velocityRef, lerpPercentage);
        transform.position = targetPos - transform.forward * zoom;
    }
}
