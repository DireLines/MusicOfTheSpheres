using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSource : MonoBehaviour {
    private SpriteRenderer sr;
    private float brightness;
    private float onBrightness = 0.7f;
    private float offBrightness = 0.2f;
    private float targetBrightness;
    private float fadeSpeed = 0.1f;//percentage 0 to 1
    // Start is called before the first frame update
    void Start() {
        //brightness = offBrightness;
        //targetBrightness = offBrightness;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        brightness += (targetBrightness - brightness) * fadeSpeed;
        sr.color = new Color(brightness, brightness, brightness, 1f);
    }

    public void PowerOn() {
        print("Power source powering on");
        brightness = 1f;
        targetBrightness = onBrightness;
    }

    public void PowerOff() {
        targetBrightness = offBrightness;
    }
}
