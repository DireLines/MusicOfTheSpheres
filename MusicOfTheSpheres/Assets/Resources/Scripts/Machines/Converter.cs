using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Basic Converter
//takes an input item at inputSlot and immediately produces an output item at outputSlot
//then has a cooldown before it can do so again
public class Converter : Machine {
    [SerializeField]
    private Item input;
    [SerializeField]
    private Item output;
    [SerializeField]
    private float cooldown;
    private InputPedestal inputSlot;
    private ItemPedestal outputSlot;



    private void Start() {
        inputSlot = transform.Find("InputSlot").gameObject.GetComponent<InputPedestal>();
        outputSlot = transform.Find("OutputSlot").gameObject.GetComponent<ItemPedestal>();
        inputSlot.machine = this;
    }

    public override void PowerOn() {
        base.PowerOn();
        //this will check to see if player placed input on slot while machine was off
        PerformMachineAction();
    }

    protected override void PerformMachineAction() {
        Item allInput = inputSlot.GetItem();
        if (allInput.HasAtLeast(input)) {
            //accept input
            Item leftovers = allInput.minus(input);
            inputSlot.SetItem(leftovers);
            //produce output
            outputSlot.Spawn(output);
            //if there is anything leftover, try to accept input again after cooldown
            if (leftovers != null) {
                Invoke("Activate", cooldown);
            }
        }
    }

    public Item GetInput() {
        return input;
    }
    public void SetInput(Item i) {
        input = i;
    }
    public Item GetOutput() {
        return output;
    }
    public void SetOutput(Item i) {
        output = i;
    }
}
