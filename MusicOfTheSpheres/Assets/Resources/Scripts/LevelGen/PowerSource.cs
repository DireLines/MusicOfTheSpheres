using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: convert to 3D equivalent of setting sprite renderer brightness
public class PowerSource : MonoBehaviour {
    //private SpriteRenderer sr;
    private float brightness;
    private float onBrightness = 0.7f;
    private float offBrightness = 0.2f;
    private float targetBrightness;
    private float fadeSpeed = 0.1f;//percentage 0 to 1

    [HideInInspector]
    public List<Machine> machines;
    // Start is called before the first frame update
    void Awake() {
        //brightness = offBrightness;
        //targetBrightness = offBrightness;
        //sr = GetComponent<SpriteRenderer>();
        machines = new List<Machine>();
    }

    // Update is called once per frame
    void Update() {
        brightness += (targetBrightness - brightness) * fadeSpeed;
        //sr.color = new Color(brightness, brightness, brightness, 1f);
    }

    public void PowerOn() {
        brightness = 1f;
        targetBrightness = onBrightness;
        foreach (Machine m in machines) {
            m.PowerOn();
        }
    }

    public void PowerOff() {
        targetBrightness = offBrightness;
        foreach (Machine m in machines) {
            m.PowerOff();
        }
    }
}
