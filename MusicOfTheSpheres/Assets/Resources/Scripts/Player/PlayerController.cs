using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    private Inventory inventory;
    private bool overPickup;
    private bool nearButton;

    [Tooltip("Movement")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    [Range(0f, 1f)]
    private float accelerationMultiplier = 0.1f;
    private Vector3 currentVelocity;
    private Vector3 targetVelocity;
    private Vector3 velocityRef;

    [Header("Interactions")]
    [SerializeField]
    private float pickupRadius;
    [SerializeField]
    private float interactRadius;


    const float ceilingHeight = 9000;
    const float reallySmall = 0.001f;
    const float stepHeight = 0.5f;

    public readonly int noteRange = 5;

    private Rigidbody rb;
    private int platformLayer;
    private int stairLayer;

    private int stippleShaderID;


    private void Start() {
        inventory = GetComponent<Inventory>();
        rb = GetComponent<Rigidbody>();

        platformLayer = LayerMask.GetMask("Platform");
        stairLayer = LayerMask.GetMask("Stair");

        stippleShaderID = Shader.PropertyToID("_PlayerPosition");
    }

    public void Move(Vector3 dir) {
        targetVelocity = dir * moveSpeed * Time.fixedDeltaTime + Vector3.up * rb.velocity.y;
        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref currentVelocity, accelerationMultiplier / 10f, moveSpeed);

        if (currentVelocity.sqrMagnitude > 0.01f * 0.01f) {
            transform.forward = Vector3.ProjectOnPlane(currentVelocity, Vector3.up);
        }

        RaycastHit hit;
        if (Physics.BoxCast(new Vector3(transform.position.x, ceilingHeight, transform.position.z), transform.localScale,
                            Vector3.down, out hit, transform.rotation, Mathf.Infinity, platformLayer | stairLayer)) {
            if (Mathf.Abs(hit.point.y - (transform.position.y - transform.localScale.y / 2f)) < transform.localScale.y) {
                rb.position = new Vector3(rb.position.x, hit.point.y + transform.localScale.y / 2f, rb.position.z);
                currentVelocity.y = 0f;
            } else {
                currentVelocity.y += Physics.gravity.y * Time.fixedDeltaTime;
            }
        } else {
            currentVelocity.y += Physics.gravity.y * Time.fixedDeltaTime;
        }

        rb.velocity = currentVelocity;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, currentVelocity * 5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, targetVelocity * 5f);
        // Gizmos.DrawWireSphere(groundCheck.position, Mathf.Min(transform.localScale.x, transform.localScale.y));
    }

    private void Update() {
        // Shader.SetGlobalVector("_PlayerPosition", new Vector4(transform.position.x, transform.position.y, transform.position.z, 1f));
    }
}
