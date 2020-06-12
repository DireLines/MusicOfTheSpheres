using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    const float reallySmall = 0.001f;

    private Transform currentPlatform;

    // Start is called before the first frame update
    void Start() {
        inventory = GetComponent<Inventory>();
        currentPlatform = null;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!currentPlatform) {
            ColumnManager CM = GameObject.Find("LevelGenerator").GetComponent<ColumnManager>();
            currentPlatform = CM.columnsInGrid[CM.PlayerGridSquare()].transform.Find("Platform");
        }
        print(currentPlatform.GetComponent<Platform>().height);
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

        Vector3 heading = transform.position + dir * Time.fixedDeltaTime;
        RaycastHit[] hits = Physics.SphereCastAll(
            heading + new Vector3(0, reallyBig, 0),
            transform.localScale.x, -Vector3.up,
            float.MaxValue,
            1 << LayerMask.NameToLayer("Platform")
        );
        hits = hits.OrderBy(hit => hit.transform.GetComponent<Platform>().height).Reverse().ToArray();
        print("current platform: " + currentPlatform.gameObject.name);
        if (hits.Length > 0) {
            Transform platform = hits[0].transform;
            //print(currentPlatform.gameObject.name + " -> " + platform.gameObject.name);
            if (platform != currentPlatform) {
                Platform p = platform.GetComponent<Platform>();
                if (Mathf.Abs(p.height - currentPlatform.GetComponent<Platform>().height) <= 1 + reallySmall) {
                    //print("success!");
                    Vector3 destination = new Vector3(heading.x, transform.localScale.y / 2 + platform.position.y + platform.localScale.y / 2 + reallySmall, heading.z);
                    transform.position = destination;
                    currentPlatform = platform;
                    return;
                } else {
                    //print("too big a gap");
                }
            } else {
                //print("same platform");
            }
        } else {
            //print("no raycast hits");
        }
        GetComponent<Rigidbody>().velocity = dir + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
    }
}
