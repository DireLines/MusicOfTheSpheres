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
        item.Collect(this);

        string key = GetKey(item);
        if (itemDict.ContainsKey(key))
        {
            itemDict[key]++;
        }
        else
        {
            itemDict.Add(key, 1);
        }
    }

    private void Remove(Item item)
    {
        item.Drop(null);

        string key = GetKey(item);
        if (itemDict.ContainsKey(key))
        {
            if (itemDict[key] > 1)
            {
                itemDict[key]--;
            } 
            else
            {
                itemDict.Remove(key);
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
