using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour {
    List<Item> items;
    Dictionary<string, int> itemDict;
    GameObject heldObject;
    int activeItemSlot;
    List<UnityEvent> addEvents;
    List<UnityEvent> removeEvents;

    private void Awake()
    {
        itemDict = new Dictionary<string, int>();
        heldObject = null;
    }

    private string GetKey(Item item) => item.stackable ? item.type.ToString() + item.category.ToString() : item.gameObject.GetInstanceID().ToString();

    public void NextItem()
    {
        SwapItem(activeItemSlot++);
    }

    public void PreviousItem()
    {
        SwapItem(activeItemSlot--);
    }

    private void SwapItem(int index)
    {
        index = index % items.Count;
        items[activeItemSlot].gameObject.SetActive(false);
        activeItemSlot = index;
        items[activeItemSlot].gameObject.SetActive(true);
    }

    private void Add(Item item)
    {
        string key = GetKey(item);
        if (itemDict.ContainsKey(key))
        {
            itemDict[key]++;
        }
        else
        {
            itemDict.Add(key, 1);
        }

        item.Collect(this);
    }

    private void Remove(Item item, bool all = false)
    {
        string key = GetKey(item);
        if (itemDict.ContainsKey(key))
        {
            if (all || itemDict[key] <= 1)
            {
                itemDict.Remove(key);
            } 
            else
            {
                itemDict[key]--;

            }
        }

        item.Drop(null);
    }

    private void RegisterEvent(ref List<UnityEvent> eventList)
    {
        if (null == eventList) eventList = new List<UnityEvent>();

        UnityEvent unityEvent = new UnityEvent();
        eventList.Add(unityEvent);
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
