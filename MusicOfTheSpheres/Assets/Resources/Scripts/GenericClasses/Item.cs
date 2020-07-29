using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory
{
    Basic,
    Special,
    Abstract,
    Machine,
};
public enum ItemType
{
    Point,
    Orb,
    Crystal,
    Wisp,
    Key,
    HealthUp,
    Machine,
    Any,
}

public class Item : MonoBehaviour, ICollectible {
    public ItemType type;
    public ItemCategory category = ItemCategory.Basic; //default to basic

    public bool stackable = true;
    public bool physical = true;
    public bool holdable = true;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void Collect (Inventory inventory)
    {
        transform.SetParent(inventory.transform.Find("ItemPoint"));
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(GameObject target)
    {
        transform.SetParent(null);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}

