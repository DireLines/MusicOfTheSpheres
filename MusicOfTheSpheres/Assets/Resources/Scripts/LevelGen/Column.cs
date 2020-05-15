using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour {
    public int note;
    public Vector2Int pos;

    private GameObject powerSource;
    public bool playNotes;
    private AudioSource asrc;
    readonly float semitone = Mathf.Pow(2f, 1f / 12);
    public int octavesDown;
    readonly float stopSpeed = 0.95f;
    private bool stopped;

    private void Awake() {
        powerSource = transform.Find("PowerSource").gameObject;
        asrc = GetComponent<AudioSource>();
    }

    private void Start() {
        asrc.pitch = Mathf.Pow(semitone, note - 12f * octavesDown);
        stopped = false;
    }

    private void Update() {
        if (stopped) {
            asrc.volume *= stopSpeed;
        }
    }

    public void PowerOn() {
        if (powerSource == null) {
            return;
        }
        powerSource.GetComponent<PowerSource>().PowerOn();
        asrc.volume = 1f;
        if (playNotes) {
            asrc.Play();
        }
        stopped = false;
    }

    public void PowerOff() {
        if (powerSource == null) {
            return;
        }
        powerSource.GetComponent<PowerSource>().PowerOff();
        stopped = true;
    }
}
