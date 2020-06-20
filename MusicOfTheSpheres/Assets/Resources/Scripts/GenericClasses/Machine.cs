using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : Item {
    public GameObject PowerIndicator;//prefab

    protected Material powerIndicator;

    [SerializeField]
    protected Color onIndicatorColor;
    [SerializeField]
    protected Color offIndicatorColor;

    protected bool powered;

    protected virtual void Awake() {
        GameObject indicatorLight = Instantiate(PowerIndicator, transform.position + new Vector3(0, transform.localScale.y / 2, 0), Quaternion.identity, transform);
        indicatorLight.name = "IndicatorLight";
        powerIndicator = indicatorLight.GetComponent<MeshRenderer>().material;
        powerIndicator.SetColor("_Color", offIndicatorColor);
    }

    protected virtual void PerformMachineAction() {
        print("Im a machine doin a thing");
    }

    public virtual void PowerOn() {
        //print("Im a generic machine powering on");
        powerIndicator.SetColor("_Color", onIndicatorColor);
        powered = true;
    }

    public virtual void PowerOff() {
        //print("Im a generic machine powering off");
        powerIndicator.SetColor("_Color", offIndicatorColor);
        powered = false;
    }

    public virtual void Activate() {
        if (powered) {
            PerformMachineAction();
        }
    }
}
