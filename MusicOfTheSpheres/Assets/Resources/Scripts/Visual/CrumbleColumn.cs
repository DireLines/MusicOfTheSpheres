﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleColumn : MonoBehaviour {

    [Tooltip("Number of seconds it takes to fully crumble the column.")]
    [Range(0f, 300f)]
    public float crumbleSeconds;

    private Transform chunks;

    private void Awake() {
        chunks = transform.Find("Chunks");
        foreach (Transform chunk in chunks) {
            if (chunk.GetComponent<Rigidbody>() != null) chunk.GetComponent<Rigidbody>().useGravity = false;
        }
        //rotate to a random right angle for visual variety
        transform.Rotate(Vector3.up, 90f * Random.Range(0, 4));
    }
    public void CrumbleBelow(float height) {
        foreach (Transform chunk in chunks) {
            Rigidbody rb = chunk.GetComponent<Rigidbody>();
            if (!rb) continue;
            if (chunk.position.y <= height && !rb.useGravity) {
                rb.useGravity = true;
                rb.angularDrag = 0f;
                rb.angularVelocity = Random.insideUnitSphere * 0.3f;
                Destroy(chunk.gameObject, 20f);
            }
        }
    }
}