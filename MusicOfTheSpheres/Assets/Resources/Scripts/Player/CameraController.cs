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
    [SerializeField]
    float lerpPercentage = 0.01f;

    private Vector3 offset;
    private Vector3 m_CurrentVelocity;
    private Vector3 targetPos;

    public Transform debugT1;
    public Transform debugT2;


    void Start() {
        target = debugT1;//debug
        turnDirection = turnInverted ? -1 : 1;
        vertDirection = vertInverted ? -1 : 1;
        targetPos = target.position;
        offset = new Vector3(targetPos.x + 6.0f, targetPos.y + 8.0f, targetPos.z + 7.0f);
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            target = (target == debugT1) ? debugT2 : debugT1;
        }

    }

    void LateUpdate() {
        //input
        offset = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * turnSpeed * turnDirection, Vector3.up) * offset;
        offset = Quaternion.AngleAxis(Input.GetAxis("Vertical") * vertSpeed * vertDirection, transform.right) * offset;
        float zoomInput = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * offsetDistance;
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
