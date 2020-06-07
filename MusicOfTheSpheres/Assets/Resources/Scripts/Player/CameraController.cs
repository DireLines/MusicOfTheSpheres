using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    float turnSpeed, vertSpeed, offsetDistance;
    [SerializeField]
    bool turnInverted, vertInverted;
    float turnDirection, vertDirection;
    [SerializeField]
    float zoomSpeed, zoomMax, zoomMin;

    [SerializeField]
    Transform target;
    public Transform Target { get => target; set => target = value; }

    [SerializeField]
    float lerpPercentage = 0.01f;

    private Vector3 offset;
    private Vector3 targetPos;


    void Start() {
        turnDirection = turnInverted ? -1 : 1;
        vertDirection = vertInverted ? -1 : 1;
        targetPos = target.position;
        offset = new Vector3(targetPos.x + 6.0f, targetPos.y + 8.0f, targetPos.z + 7.0f);
    }
    void LateUpdate() {
        //input
        offset = Quaternion.AngleAxis(Input.GetAxis("Camera Horizontal") * turnSpeed * turnDirection * Time.deltaTime, Vector3.up) * offset;
        offset = Quaternion.AngleAxis(Input.GetAxis("Camera Vertical") * vertSpeed * vertDirection * Time.deltaTime, transform.right) * offset;
        float zoomInput = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * offsetDistance * Time.deltaTime;
        offsetDistance = Mathf.Clamp(offsetDistance + zoomInput, zoomMin, zoomMax);

        //clamp vertical angle to reasonable region
        //TODO: bound above & below by angle and not y value
        //float yDiff = offset.y - targetPos.y;
        //offset = new Vector3(offset.x, targetPos.y + Mathf.Max(0, yDiff), offset.z);

        //renormalize offset so that it's the proper distance away
        offset = offset.normalized * offsetDistance;

        transform.position = targetPos + offset;
        transform.LookAt(targetPos);

        //exp lerp toward new targets
        targetPos = lerpPercentage * (target.position - targetPos) + targetPos;
    }
}
