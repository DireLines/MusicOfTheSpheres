using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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
    [SerializeField]
    private Transform groundCheck;

    const float ceilingHeight = 9000;
    const float reallySmall = 0.001f;
    const float stepHeight = 0.5f;

    public readonly int noteRange = 5;

    private Rigidbody rb;
    private int platformLayer;
    private int stairLayer;

    Vector3 dir;

    private void Start() {
        inventory = GetComponent<Inventory>();
        rb = GetComponent<Rigidbody>();

        platformLayer = LayerMask.GetMask("Platform");
        stairLayer = LayerMask.GetMask("Stair");

        if (groundCheck == null) {
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null) {
                Debug.LogError("No GroundCheck found on Player");
            }
        }
    }

    public void Move (Vector3 dir) {
        Vector3 velocity = dir * moveSpeed * Time.fixedDeltaTime + Vector3.up * rb.velocity.y;
        if (dir.magnitude > 0) {
            transform.forward = dir;
        }
        this.dir = dir;

        RaycastHit hit;
        if (Physics.BoxCast(transform.position + Vector3.up * ceilingHeight, transform.localScale, 
                            Vector3.down, out hit, transform.rotation, Mathf.Infinity, platformLayer | stairLayer)) {
            if (Mathf.Abs(hit.point.y - groundCheck.position.y) < transform.localScale.y) {
                rb.position = new Vector3(rb.position.x, hit.point.y + transform.localScale.y / 2f, rb.position.z);
                velocity.y = 0f;
            } else {
                velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
            }
        } else {
            velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
        }

        rb.velocity = velocity;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, dir * 5f);
        // Gizmos.DrawWireSphere(groundCheck.position, Mathf.Min(transform.localScale.x, transform.localScale.y));
    }
}
