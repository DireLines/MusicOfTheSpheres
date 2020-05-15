using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    float turnSpeed, vertSpeed, offsetDistance;

    [SerializeField]
    Transform target;

    private Vector3 offset;


    void Start() {
        offset = new Vector3(target.position.x + 6.0f, target.position.y + 8.0f, target.position.z + 7.0f);
    }

    void LateUpdate() {
        //input
        offset = Quaternion.AngleAxis(-Input.GetAxisRaw("Horizontal") * turnSpeed, Vector3.up) * offset;
        offset = Quaternion.AngleAxis(Input.GetAxisRaw("Vertical") * vertSpeed, transform.right) * offset;

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
