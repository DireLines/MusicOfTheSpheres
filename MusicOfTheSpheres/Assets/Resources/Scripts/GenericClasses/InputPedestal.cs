using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPedestal : ItemPedestal, Interactable {
    public Machine machine; //the machine this is an input to
    public void Interact(GameObject interactor) {
        machine.Activate();
        ////player (or robot) has just pressed this input slot
        ////get its inventory
        //Inventory inv = interactor.GetComponent<Inventory>();
        //if (inv) {
        //    //if they have all of the appropriate items, accept them
        //    //otherwise, play a buzzer noise
        //    foreach (Item i in machine.requiredInputs) {
        //        if (!inv.Contains(i)) {
        //            //play buzzer sound
        //            return;
        //        }
        //    }
        //    foreach (Item i in machine.requiredInputs) {

        //    }
        //}
    }

    public void PlaceItem(GameObject item) {
        //TODO: make the item sit on top of the pedestal
        item.transform.position = transform.position;
    }

}
