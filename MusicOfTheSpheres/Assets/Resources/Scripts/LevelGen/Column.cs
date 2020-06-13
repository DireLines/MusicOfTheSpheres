using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour {
    public int note;
    public int worldHeight;
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
    private float onBrightness = 0.7f;
    private float offBrightness = 0.4f;
    private float targetBrightness;
    private float fadeSpeed = 0.085f;//percentage 0 to 1

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
        asrc = GetComponent<AudioSource>();
    }

    private void Start() {
        asrc.pitch = Mathf.Pow(semitone, note - 12f * octavesDown);
        stopped = false;
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
}
