using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Room : MonoBehaviour {
    public int note;
    public Tuple<int, int> pos;

    private GameObject powerSource;
    private AudioSource asrc;
    readonly float semitone = Mathf.Pow(2f, 1f / 12);
    readonly int octavesDown = 5;

    private void Awake() {
        powerSource = transform.Find("PowerSource").gameObject;
        asrc = GetComponent<AudioSource>();
    }

    private void Start() {
        asrc.pitch = Mathf.Pow(semitone, note - 12f * octavesDown);
    }

    public void PowerOn() {
        if (powerSource == null) {
            return;
        }
        powerSource.GetComponent<PowerSource>().PowerOn();
        asrc.Play();
    }

    public void PowerOff() {
        if (powerSource == null) {
            return;
        }
        powerSource.GetComponent<PowerSource>().PowerOff();
    }
}
