using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    private PlayerController controller;

    Transform cam;

    Vector3 rightDirection;
    Vector3 forwardDirection;
    Vector3 direction;

    float right;
    float forward;

    private void Start() {
        controller = GetComponent<PlayerController>();
        cam = Camera.main.transform;
    }

    private void FixedUpdate() {
        right = Input.GetAxisRaw("Player Horizontal");
        forward = Input.GetAxisRaw("Player Vertical");
        rightDirection = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
        forwardDirection = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;

        direction = rightDirection * right + forwardDirection * forward;
        if (direction.sqrMagnitude > 1f) {
            direction.Normalize();
        }
        print(direction);
        controller.Move(direction);
    }
}
