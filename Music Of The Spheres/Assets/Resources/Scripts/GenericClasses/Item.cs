using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public ItemType type;
    public ItemCategory category = ItemCategory.Basic; //default to basic
    public Sprite icon;

    public int amount = 1;
    public bool stackable = true;

    public bool physical;
    public bool holdable;

    public Item(Item other) {
        type = other.type;
        category = other.category;
        icon = other.icon;
        amount = other.amount;
        stackable = other.stackable;
        physical = other.physical;
        holdable = other.holdable;
    }
}

