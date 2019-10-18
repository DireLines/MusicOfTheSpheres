using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//all enums go here
public enum ItemCategory {
    Basic,
    Special,
    Abstract,
    Machine,
};
public enum ItemType {
    Point,
    Orb,
    Crystal,
    Wisp,
    Key,
    HealthUp,
    Machine,
    Any,
};
//keeping layer orders consistent
public enum Height {
    Floor = -10,
    OnFloor = 0,
    Player = 1,
    OnPedestal = 2,
    Held = 3,
    Wall = 4,
    Floating = 10,
}

//important globals and globally accessible functions
public static class Game {
    public static bool IsOnOSX = (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer);
    public static bool IsOnWindows = (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer);
    public static bool IsOnLinux = (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.LinuxPlayer);

    public static Collider2D[] ItemsAround(Vector3 pos, float radius) {
        return Physics2D.OverlapCircleAll(pos, radius);
    }
    public static Collider2D[] InteractablesAround(Vector3 pos, float radius) {
        return Physics2D.OverlapCircleAll(pos, radius, 1 << LayerMask.NameToLayer("Interactable"));
    }

    //called on an object and recursively sets its and its children's sorting order to be based on the desired Height
    public static void RepositionHeight(GameObject obj, Height height) {
        RepositionInSortingOrder(obj, (int)height);
    }
    private static void RepositionInSortingOrder(GameObject obj, int height) {
        int originalSortingOrder = obj.GetComponent<SpriteRenderer>().sortingOrder;
        obj.GetComponent<SpriteRenderer>().sortingOrder = height;
        foreach (Transform child in obj.transform) {
            int diff = child.gameObject.GetComponent<SpriteRenderer>().sortingOrder - originalSortingOrder;
            RepositionInSortingOrder(child.gameObject, height + diff);
        }
    }

    //C# mod is not too useful. This one acts identically to the python one (and the math one)
    public static int correctmod(int a, int n) {
        return ((a % n) + n) % n;
    }

    public static void DisablePhysics(GameObject obj) {
        if (obj.GetComponent<BoxCollider2D>() != null) {
            obj.GetComponent<BoxCollider2D>().enabled = false;
        }
        if (obj.GetComponent<Rigidbody2D>() != null) {
            obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            obj.GetComponent<Rigidbody2D>().isKinematic = true;
            obj.GetComponent<Rigidbody2D>().freezeRotation = true;
        }
    }

    public static void EnablePhysics(GameObject obj) {
        if (obj.GetComponent<BoxCollider2D>() != null) {
            obj.GetComponent<BoxCollider2D>().enabled = true;
        }
        if (obj.GetComponent<Rigidbody2D>() != null) {
            obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            obj.GetComponent<Rigidbody2D>().isKinematic = false;
            obj.GetComponent<Rigidbody2D>().freezeRotation = false;
        }
    }

    //set layer recursively
    public static void SetLayer(GameObject obj, LayerMask layer) {
        obj.layer = layer;
        foreach (Transform child in obj.transform) {
            SetLayer(child.gameObject, layer);
        }
    }
}
