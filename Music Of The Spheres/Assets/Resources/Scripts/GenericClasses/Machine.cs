using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : Item {

    protected SpriteRenderer powerIndicator;

    [SerializeField]
    protected Color onIndicatorColor;
    [SerializeField]
    protected Color offIndicatorColor;

    protected bool powered;

    protected virtual void Awake() {
        powerIndicator = transform.Find("IndicatorLight").gameObject.GetComponent<SpriteRenderer>();
    }
    // Use this for initialization
    protected virtual void Start() {
        //powerIndicator = transform.Find("IndicatorLight").gameObject.GetComponent<SpriteRenderer>();
    }

    protected virtual void PerformMachineAction() {
        print("Im a machine doin a thing");
    }

    public virtual void PowerOn() {
        //print("Im a generic machine powering on");
        powerIndicator.color = onIndicatorColor;
        powered = true;
    }

    public virtual void PowerOff() {
        //print("Im a generic machine powering off");
        powerIndicator.color = offIndicatorColor;
        powered = false;
    }

    public virtual void Activate() {
        if (powered) {
            PerformMachineAction();
        }
    }


}
