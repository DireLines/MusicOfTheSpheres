using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour {
    public int note;
    public int worldHeight;
    public Vector2Int pos;
    private GameObject powerSource;

    //note play
    public bool playNotes;
    private AudioSource asrc;
    readonly float semitone = Mathf.Pow(2f, 1f / 12);
    public int octavesDown;
    readonly float stopSpeed = 0.95f;
    private bool stopped;

    //color change
    private Material columnMat;
    private Color initColor;
    public Color color { get => initColor; set => initColor = value; }
    private float brightness;
    private float onBrightness = 0.7f;
    private float offBrightness = 0.4f;
    private float targetBrightness;
    private float fadeSpeed = 0.085f;//percentage 0 to 1

    private void Awake() {
        brightness = offBrightness;
        targetBrightness = offBrightness;
        columnMat = transform.Find("Column").GetComponent<MeshRenderer>().material;
        initColor = columnMat.GetColor("_Color");
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
        brightness += (targetBrightness - brightness) * fadeSpeed;
        Color c = initColor * brightness;
        columnMat.SetColor("_Color", c);
    }

    public void PowerOn() {
        if (powerSource == null) {
            return;
        }
        brightness = 1f;
        targetBrightness = onBrightness;
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
        targetBrightness = offBrightness;
        powerSource.GetComponent<PowerSource>().PowerOff();
        stopped = true;
    }
}
