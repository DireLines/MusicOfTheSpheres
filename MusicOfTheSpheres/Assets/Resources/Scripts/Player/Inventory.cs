using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    List<Item> items;
    Dictionary<string, int> itemDict;
    GameObject heldObject;
    int activeItemSlot;

    private void Awake()
    {
        itemDict = new Dictionary<string, int>();
        heldObject = null;
    }

    private string GetKey(Item item) => item.stackable ? item.type.ToString() + item.category.ToString() : item.gameObject.GetInstanceID().ToString();

    private void Add(Item item)
    {
        string key = GetKey(item);
        item.Collect(this);
    }

    private void Remove(Item item)
    {
        string key = GetKey(item);
        if (itemDict.ContainsKey(key))
        {
            if (itemDict[key] > 1)
            {

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (null != item)
        {
            Add(item);
        }
    }
}
