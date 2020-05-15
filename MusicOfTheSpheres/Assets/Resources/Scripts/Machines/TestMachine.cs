using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMachine : Machine {
    public override void PowerOn() {
        //print("Im a test machine powering on");
        base.PowerOn();
    }

    public override void PowerOff() {
        //print("Im a test machine powering off");
        base.PowerOff();
    }
}
