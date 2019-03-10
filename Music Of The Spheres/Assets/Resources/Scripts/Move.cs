using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    public float speed;
    private Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void FixedUpdate() {
        Vector2 vel = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (vel.sqrMagnitude > 1f) {
            vel.Normalize();
        }
        rb.velocity = vel * speed;
    }
}
