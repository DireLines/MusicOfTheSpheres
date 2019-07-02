using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    List<Item> items;
    GameObject heldItem;
    // Start is called before the first frame update
    void Start() {
        items = new List<Item>();
    }

    public Item combine(Item a, Item b) {
        Item c = new Item(a);
        if (a.type == b.type) {
            c.amount += b.amount;
        }
        return c;
    }
}
