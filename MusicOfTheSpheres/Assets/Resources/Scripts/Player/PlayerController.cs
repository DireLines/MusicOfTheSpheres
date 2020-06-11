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

    const float reallyBig = 9000;
    const float reallySmall = 0.0001f;

    private Platform currentPlatform;

    // Start is called before the first frame update
    void Start() {
        inventory = GetComponent<Inventory>();
        RaycastHit[] hits = Physics.RaycastAll(transform.position + new Vector3(0, reallyBig, 0), -Vector3.up, float.MaxValue, LayerMask.GetMask("Platform"));
        currentPlatform = hits[0].transform.GetComponent<Platform>();
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
        //TODO: stair movement

        Vector3 heading = transform.position + dir;
        RaycastHit[] hits = Physics.RaycastAll(heading + new Vector3(0, reallyBig, 0), -Vector3.up, float.MaxValue, LayerMask.GetMask("Platform"));
        if (hits.Length < 1) {
            GetComponent<Rigidbody>().velocity = dir + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
            return;
        }
        Transform platform = hits[0].transform;
        if (platform == currentPlatform.transform) {
            GetComponent<Rigidbody>().velocity = dir + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
            return;
        }
        Platform p = platform.GetComponent<Platform>();
        if (Mathf.Abs(p.height - currentPlatform.height) > 5) {
            GetComponent<Rigidbody>().velocity = dir + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
            return;
        }
        print("shibble dibble");
        GetComponent<Rigidbody>().velocity = dir;
        transform.position.Set(heading.x, platform.localPosition.y + platform.localScale.y / 2 + reallySmall, heading.z);
        currentPlatform = p;
    }
}
