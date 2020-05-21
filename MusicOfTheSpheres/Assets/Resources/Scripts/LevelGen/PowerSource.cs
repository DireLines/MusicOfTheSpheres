using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSource : MonoBehaviour {

    [HideInInspector]
    public List<Machine> machines;
    // Start is called before the first frame update
    void Awake() {
        machines = new List<Machine>();
    }


    public void PowerOn() {
        foreach (Machine m in machines) {
            m.PowerOn();
        }
    }

    public void PowerOff() {
        foreach (Machine m in machines) {
            m.PowerOff();
        }
    }
}
