using System;
using UnityEngine;

public class Camera2DFollow : MonoBehaviour {
    public Transform target;
    public float damping = 1;

    private float m_OffsetZ;
    private Vector3 m_CurrentVelocity;
    private Vector3 m_LookAheadPos;

    [SerializeField]
    float zoomSpeed, zoomMax, zoomMin;

    // Use this for initialization
    private void Start() {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        m_OffsetZ = (transform.position - target.position).z;
        transform.parent = null;
    }

    // Update is called once per frame
    private void Update() {

        Vector3 aheadTargetPos = target.position + Vector3.forward * m_OffsetZ;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

        transform.position = newPos;

        //zoom with mouse wheel
        float zoomInput = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * GetComponent<Camera>().orthographicSize;
        GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize + zoomInput, zoomMin, zoomMax);
    }
}
