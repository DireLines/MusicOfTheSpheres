using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Inventory inventory;
    private bool overPickup;
    private bool nearButton;

    [SerializeField]
    private float pickupRadius;
    [SerializeField]
    private float interactRadius;
    // Start is called before the first frame update
    void Start() {
        inventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update() {

    }
}
