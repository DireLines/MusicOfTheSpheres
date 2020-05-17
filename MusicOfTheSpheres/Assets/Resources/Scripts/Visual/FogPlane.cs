using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogPlane : MonoBehaviour {
    Transform mainCam;

    [SerializeField]
    float offset, maxDistBelowTarget;

    private void Start() {
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update() {
        float height = Mathf.Min(mainCam.position.y + offset, mainCam.GetComponent<CameraController>().Target.position.y - maxDistBelowTarget);
        transform.position = new Vector3(mainCam.position.x, height, mainCam.position.z);
    }
}
