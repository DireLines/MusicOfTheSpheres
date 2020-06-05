using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Inventory inventory;
    private bool overPickup;
    private bool nearButton;

    [SerializeField]
    private float pickupRadius;
    [SerializeField]
    private float interactRadius;
    [SerializeField]
    private float moveSpeed;

    // Start is called before the first frame update
    void Start() {
        inventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
        Vector2 movementWithinPlane = new Vector2(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
        if (movementWithinPlane.magnitude > moveSpeed) {
            movementWithinPlane = movementWithinPlane.normalized * moveSpeed;
        }
        Vector3 dir = forward * movementWithinPlane.y + right * movementWithinPlane.x;
        if (dir.magnitude > 0) {
            transform.forward = dir;
        }
        GetComponent<Rigidbody>().velocity = dir + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
    }
}
