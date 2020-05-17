using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogPlane : MonoBehaviour {
    Transform mainCam;

    [SerializeField]
    float offset;

    private void Start() {
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update() {
        transform.position = mainCam.position + new Vector3(0, offset, 0);
    }
}
