using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpcomingNote : MonoBehaviour {

    //List<TrailRenderer> trails;
    //private void Start() {
    //    trails = new List<TrailRenderer>(GetComponentsInChildren<TrailRenderer>());
    //}
    void Update() {
        transform.position += Vector3.up * Time.deltaTime;
    }
}
