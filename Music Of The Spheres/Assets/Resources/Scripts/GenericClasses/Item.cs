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

    public bool Sametype(Item other) {
        return type == other.type;
    }
    public bool HasAtLeast(Item other) {
        return Sametype(other) && (amount >= other.amount);
    }
    public Item plus(Item other) {
        if (type != other.type) {
            print("tried to combine items of different type");
            return this;
        }
        amount += other.amount;
        return this;
    }

    public Item minus(Item other) {
        if (type == other.type) {
            if (amount < other.amount) {
                print("tried to remove too many of an item");
                return null;
            }
            amount -= other.amount;
        }
        if (amount == 0) {
            //used up all of the item
            //TODO: figure out if this should be null or some special Item called "no item"
            return null;
        }
        return this;
    }

}

