using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    List<Item> items;
    GameObject heldObject;
    // Start is called before the first frame update
    void Start() {
        items = new List<Item>();
    }

    public void Add(Item item) {
        if (item == null) {
            return;
        }
        if (item.stackable) {
            foreach (Item i in items) {
                if (item.Sametype(i)) {
                    items.Remove(i);
                    items.Add(i.plus(item));
                    return;
                }
            }
        }
        //if you didn't have the item or it was not stackable, just add it
        items.Add(item);
    }

    public void Hold(GameObject obj) {
        Item item = obj.GetComponent<Item>();
        if (!item) {
            print("tried to hold a non-item");
            return;
        }
        if (!item.holdable) {
            print("tried to hold a non-holdable item");
            return;
        }

        heldObject = obj;
        obj.GetComponent<SpriteRenderer>().sortingOrder = (int)Height.Held;
        obj.transform.parent = transform;
        obj.transform.position = transform.position;
    }

    public bool Contains(Item item) {
        if (heldObject.GetComponent<Item>().HasAtLeast(item)) {
            return true;
        }
        foreach (Item i in items) {
            if (i.HasAtLeast(item)) {
                return true;
            }
        }
        return false;
    }
}
