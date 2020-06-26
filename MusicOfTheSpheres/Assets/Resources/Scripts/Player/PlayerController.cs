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

    public readonly int noteRange = 5;

    // Start is called before the first frame update
    void Start() {
        inventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
        Vector2 movementWithinPlane = new Vector2(Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime, Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime);
        if (movementWithinPlane.magnitude > moveSpeed) {
            movementWithinPlane = movementWithinPlane.normalized * moveSpeed;
        }
        Vector3 dir = forward * movementWithinPlane.y + right * movementWithinPlane.x;
        if (dir.magnitude > 0) {
            transform.forward = dir;
        }


        Transform currentPlatform = getCurrentPlatform();
        Vector3 heading = transform.position + dir * Time.fixedDeltaTime;
        RaycastHit[] hits = Physics.BoxCastAll(
            heading + new Vector3(0, reallyBig, 0),
            transform.localScale / 2, -Vector3.up,
            transform.rotation, float.MaxValue,
            LayerMask.GetMask("Platform")
        );
        hits = hits.
            OrderBy(hit => hit.transform.GetComponent<Platform>().height).
            Reverse().ToArray();
        if (hits.Length > 0) {
            Transform platform = hits[0].transform;
            //if (platform == currentPlatform) { GetComponent<Rigidbody>().velocity = dir + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0); return; }
            Platform p = platform.GetComponent<Platform>();
            if (Mathf.Abs(p.height - currentPlatform.GetComponent<Platform>().height) <= 1) {
                Vector3 destination = new Vector3(heading.x, transform.localScale.y / 2 + platform.position.y + platform.localScale.y / 2 + reallySmall, heading.z);
                transform.position = destination;
                return;
            }
        }
        GetComponent<Rigidbody>().velocity = dir + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
    }

    Transform getCurrentPlatform() {
        RaycastHit[] platformsBelow = Physics.RaycastAll(
            transform.position + new Vector3(0, reallyBig, 0),
            -Vector3.up,
            float.MaxValue,
            LayerMask.GetMask("Platform")
        );
        platformsBelow = platformsBelow.OrderBy(hit => hit.transform.GetComponent<Platform>().height).Reverse().ToArray();
        if (platformsBelow.Length > 0) {
            return platformsBelow[0].transform;
        } else {
            ColumnManager CM = GameObject.Find("LevelGenerator").GetComponent<ColumnManager>();
            Vector2Int pgs = CM.PlayerGridSquare();
            if (CM.columnsInGrid.ContainsKey(pgs)) {
                return CM.columnsInGrid[pgs].transform.Find("Platform");
            } else {
                return null;
            }
        }
    }
}
