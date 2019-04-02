using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Room : MonoBehaviour {
    public int note;
    public Tuple<int, int> pos;

    public GameObject powerSource;

    private void Start() {
        //powerSource = transform.Find("PowerSource").gameObject;
    }

    public void PowerOn() {
        print("Room powering on");
        if (powerSource == null) {
            return;
        }
        powerSource.GetComponent<PowerSource>().PowerOn();
    }

    public void PowerOff() {
        print("Room powering off");
        if (powerSource == null) {
            return;
        }
        powerSource.GetComponent<PowerSource>().PowerOff();
    }
}
