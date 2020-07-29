using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: figure out what positions to spawn things at
public class ItemPedestal : MonoBehaviour {
    private SpriteRenderer iconSR;
    private Item heldItem;

    public void Start() {
        heldItem = null;
        iconSR = GetComponent<SpriteRenderer>();
    }
    public void PlaceItem(GameObject item) {
        float x = transform.position.x;
        float y = transform.position.y;
        item.transform.position = new Vector3(x, y, item.transform.position.z);
    }

    public void SpawnPhysicalItem(GameObject item) {
        float x = transform.position.x;
        float y = transform.position.y;
        Vector3 pos = new Vector3(x, y, item.transform.position.z);
        GameObject newItem = Instantiate(item, pos, Quaternion.identity, null);
    }


    public Item GetItem() {
        return heldItem;
    }
    public void SetItem(Item item) {
        heldItem = item;
        //iconSR.sprite = item.icon;
    }
    public void Spawn(Item item) {
        if (item.physical && item.gameObject) {
            //spawn the physical item
            GameObject obj = item.gameObject;
            print("Spawning a physical version of the item");
            SpawnPhysicalItem(obj);
            return;
        }
        //if not a physical item, spawn only using by changing the icon
        print("ItemPedestal spawning the item");
    }

    //public void
}