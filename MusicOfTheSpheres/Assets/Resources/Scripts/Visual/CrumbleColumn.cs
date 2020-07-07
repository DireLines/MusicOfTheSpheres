using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleColumn : MonoBehaviour {

    [Tooltip("Number of seconds it takes to fully crumble the column.")]
    [Range(0f, 300f)]
    public float crumbleSeconds;

    private Transform chunks;

    //example usage
    //private float crumbleHeight;

    private void Awake() {
        //crumbleHeight = 0;
        chunks = transform.Find("Chunks");
        foreach (Transform piece in chunks) {
            if (piece.GetComponent<Rigidbody>() != null) piece.GetComponent<Rigidbody>().useGravity = false;
        }
        //rotate to a random right angle for visual variety
        transform.Rotate(Vector3.up, 90f * Random.Range(0, 4));
    }

    private void Update() {
        //if (Input.GetKey(KeyCode.Space)) {
        //crumbleHeight += Time.deltaTime / crumbleSeconds * 256;
        //}
        //CrumbleBelow(crumbleHeight);
    }

    public void CrumbleBelow(float height) {
        foreach (Transform piece in chunks) {
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (!rb) continue;
            if (piece.position.y <= height && !rb.useGravity) {
                rb.useGravity = true;
                rb.angularDrag = 0f;
                rb.angularVelocity = Random.insideUnitSphere * 0.3f;
                Destroy(piece.gameObject, 20f);
            }
        }
    }
}