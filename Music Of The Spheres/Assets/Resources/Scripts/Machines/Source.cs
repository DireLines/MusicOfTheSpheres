using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : Machine {
    [SerializeField]
    private Item item;
    [SerializeField]
    private float cooldown;
    private ItemPedestal outputSlot;

    private float cooldownTimer;


    private void Start() {
        outputSlot = transform.Find("OutputSlot").gameObject.GetComponent<ItemPedestal>();
        cooldownTimer = cooldown;
    }

    private void Update() {
        if (powered) {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0) {
                cooldownTimer = cooldown;
                //spawn the item
                outputSlot.spawn(item);
                print("source machine \"spawning the item\"");
            }
        }
    }

    public Item GetItem() {
        return item;
    }

    public void SetItem(Item i) {
        item = i;
    }
}