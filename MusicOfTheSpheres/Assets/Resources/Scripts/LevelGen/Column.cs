﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Column : MonoBehaviour {
    public int note;
    public Vector2Int pos;

    //note play
    public bool playNotes;
    private AudioSource asrc;
    readonly float semitone = Mathf.Pow(2f, 1f / 12);
    public int octavesDown;
    readonly float stopSpeed = 0.95f;
    private bool stopped;

    //color change
    private List<Material> columnMat;
    private Color initColor;
    public Color color { get => initColor; set => initColor = value; }
    private float brightness;
    const float onBrightness = 0.7f;
    const float offBrightness = 0.4f;
    private float targetBrightness;
    const float fadeSpeed = 0.085f;//percentage 0 to 1

    //display upcoming notes
    const float midiDisplayScale = 1f;
    const float falloffDistance = 10f;
    List<MidiEvent> upcomingEvents;
    private MidiEventHandler MEH;
    private List<Transform> chunks;

    [HideInInspector]
    public List<Machine> machines;

    private void Awake() {
        machines = new List<Machine>();
        brightness = offBrightness;
        targetBrightness = offBrightness;
        columnMat = new List<Material>();
        columnMat.Add(transform.Find("Platform").GetComponent<MeshRenderer>().material);
        foreach (MeshRenderer mr in transform.Find("Stairs").GetComponentsInChildren<MeshRenderer>()) {
            columnMat.Add(mr.material);
        }
        initColor = columnMat[0].GetColor("_Color");
        transform.Find("Column").GetComponent<MeshRenderer>().material.SetColor("_Color", initColor * offBrightness);
        foreach (MeshRenderer mr in transform.Find("Crumbling Column").GetComponentsInChildren<MeshRenderer>()) {
            mr.material.SetColor("_Color", initColor * offBrightness);
        }
        asrc = GetComponent<AudioSource>();
    }

    private void Start() {
        asrc.pitch = Mathf.Pow(semitone, note - 12f * octavesDown);
        stopped = false;
        MEH = GameObject.Find("LevelGenerator").GetComponent<MidiEventHandler>();
        upcomingEvents = MEH.EventsForNote(note);
    }

    private void Update() {
        columnMat = new List<Material>();
        columnMat.Add(transform.Find("Platform").GetComponent<MeshRenderer>().material);
        foreach (MeshRenderer mr in transform.Find("Stairs").GetComponentsInChildren<MeshRenderer>()) {
            columnMat.Add(mr.material);
        }
        if (stopped) {
            asrc.volume *= stopSpeed;
        }
        brightness += (targetBrightness - brightness) * fadeSpeed;
        Color c = initColor * brightness;
        foreach (Material m in columnMat) {
            m.SetColor("_Color", c);
        }

        //upcoming event display
        float columnTop = transform.Find("Platform").position.y - transform.Find("Platform").localScale.y / 2;
    }

    public void PowerOn() {
        brightness = 1f;
        targetBrightness = onBrightness;
        foreach (Machine m in machines) {
            m.PowerOn();
        }
        asrc.volume = 1f;
        if (playNotes) {
            asrc.Play();
        }
        stopped = false;
    }

    public void PowerOff() {
        targetBrightness = offBrightness;
        foreach (Machine m in machines) {
            m.PowerOff();
        }
        stopped = true;
    }
    float heightForMidiTime(float eventTime, float columnTop, float elapsedMIDITime) {
        return columnTop + midiDisplayScale * (elapsedMIDITime - eventTime);
    }

    float falloffBrightness(float chunkHeight, float eventHeight, float falloff) {
        float diff = Mathf.Abs(chunkHeight - eventHeight);
        return Mathf.Lerp(onBrightness, offBrightness, diff / falloff);
    }
}
