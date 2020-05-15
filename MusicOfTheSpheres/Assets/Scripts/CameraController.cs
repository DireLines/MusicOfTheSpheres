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
    public Transform Target {
        get { return target; }
        set { target = value; }
    }

    private Vector3 offset;


    void Start() {
        turnDirection = turnInverted ? -1 : 1;
        vertDirection = vertInverted ? -1 : 1;
        offset = new Vector3(target.position.x + 6.0f, target.position.y + 8.0f, target.position.z + 7.0f);
    }

    void LateUpdate() {
        //input
        offset = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * turnSpeed * turnDirection, Vector3.up) * offset;
        offset = Quaternion.AngleAxis(Input.GetAxis("Vertical") * vertSpeed * vertDirection, transform.right) * offset;
        float zoomInput = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * offsetDistance;
        offsetDistance = Mathf.Clamp(offsetDistance + zoomInput, zoomMin, zoomMax);

        //clamp vertical angle to reasonable region
        //TODO: bound above & below by angle and not y value
        float yDiff = offset.y - target.position.y;
        offset = new Vector3(offset.x, target.position.y + Mathf.Max(0, yDiff), offset.z);

        //renormalize offset so that it's the proper distance away
        offset = offset.normalized * offsetDistance;

        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }
}
