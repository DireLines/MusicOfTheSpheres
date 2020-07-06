using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleColumn : MonoBehaviour {

    [Tooltip("Number of seconds it takes to fully crumble the column.")]
    [Range(0f, 300f)]
    public float crumbleSeconds;

    private void Awake() {
        foreach (Transform piece in transform) {
            if (piece.GetComponent<Rigidbody>() != null) piece.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) Crumble();
    }

    public void Crumble () {
        foreach (Transform piece in transform) {
            if (piece.GetComponent<Rigidbody>() == null) continue;
            StartCoroutine(CrumbleCR(piece));
        }
    }

    private IEnumerator CrumbleCR (Transform piece) {
        Rigidbody rb = piece.GetComponent<Rigidbody>();
        float t = piece.localPosition.y / 256f;
        float waitTime = Mathf.Lerp(0f, 60f, t);
        yield return new WaitForSeconds(waitTime);
        print($"{piece.name} crumbled after {waitTime} seconds.");
        rb.useGravity = true;
        rb.angularDrag = 0f;
        rb.angularVelocity = Random.insideUnitSphere * 0.3f;
        Destroy(piece.gameObject, 20f);
    }
}