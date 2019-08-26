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
    public static Collider2D[] ItemsAround(Vector3 pos, float radius) {
        return Physics2D.OverlapCircleAll(pos, radius);
    }
    public static Collider2D[] InteractablesAround(Vector3 pos, float radius) {
        return Physics2D.OverlapCircleAll(pos, radius);
    }
}
