using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomMaker : MonoBehaviour {
    public GameObject player;
    public GameObject NoteRoom;//room prefab
    public GameObject Wall;//wall prefab
    public GameObject GenericMachine;//test machine prefab
    private Dictionary<int, GameObject> rooms;//all note rooms this script has generated, indexed by note
    private Dictionary<Vector2Int, GameObject> roomsInGrid; //all note rooms this script has generated, indexed by position in grid

    public MidiEventHandler MEH;

    private IEnumerator coroutine;


    //grid settings
    private float cellSize;

    private bool putANote = false;
    private int baseNote;

    // Start is called before the first frame update
    void Start() {
        cellSize = NoteRoom.transform.Find("Cell").lossyScale.x;
        rooms = new Dictionary<int, GameObject>();
        roomsInGrid = new Dictionary<Vector2Int, GameObject>();
    }

    public void DestroyNoteRoom(int note) {
        //print("destroying room " + note);
        PowerOff(note);
        //rooms[note].GetComponent<SpriteRenderer>().enabled = false;
        //rooms[note].GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
        rooms[note].transform.Find("Void").gameObject.SetActive(true);
        //RemoveWalls(rooms[note].GetComponent<Room>().pos);
    }

    public void PowerOn(int note) {
        rooms[note].GetComponent<Room>().PowerOn();
    }

    public void PowerOff(int note) {
        rooms[note].GetComponent<Room>().PowerOff();
    }


    //create normal note room which responds to note at closest unoccupied grid square to the player
    public void CreateNoteRoom(int note) {
        Vector2Int gridPosition = ClosestUnoccupiedGridSquare(player.transform.position);
        Vector3 worldPosition = GridToWorldPosition(gridPosition, player.transform.position.z);
        GameObject newRoom = Instantiate(NoteRoom, worldPosition, Quaternion.identity, gameObject.transform);
        newRoom.name = "Room " + note;
        rooms[note] = newRoom;
        newRoom.GetComponent<Room>().note = note;
        roomsInGrid[gridPosition] = newRoom;
        newRoom.GetComponent<Room>().pos = gridPosition;
        for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++) {
            GameObject machine = Instantiate(
                GenericMachine,
                newRoom.transform.position +
                new Vector3(UnityEngine.Random.Range(-cellSize / 4, cellSize / 4),
                            UnityEngine.Random.Range(-cellSize / 4, cellSize / 4),
                            0f),
                Quaternion.identity,
                newRoom.transform);
            newRoom.transform.Find("PowerSource").GetComponent<PowerSource>().machines.Add(machine.GetComponent<Machine>());
        }
        float degree = (note - 32) / 81f;
        newRoom.transform.Find("Floor").GetComponent<SpriteRenderer>().color = new Color(degree, 0f, (1f - degree) * 0.3f, 1f);
        //figure out which wall to remove from this room
        //if the player is in one of the adjacent squares, remove only the wall for that one
        int x = gridPosition.x;
        int y = gridPosition.y;
        //print(x + ", " + y);
        Vector2Int up = new Vector2Int(x, y + 1);
        Vector2Int down = new Vector2Int(x, y - 1);
        Vector2Int left = new Vector2Int(x - 1, y);
        Vector2Int right = new Vector2Int(x + 1, y);
        Vector2Int playerSquare = PlayerGridSquare();
        if (playerSquare.x == up.x && playerSquare.y == up.y) {
            RemoveUp(gridPosition);
            return;
        }
        if (playerSquare.x == down.x && playerSquare.y == down.y) {
            RemoveDown(gridPosition);
            return;
        }
        if (playerSquare.x == left.x && playerSquare.y == left.y) {
            RemoveLeft(gridPosition);
            return;
        }
        if (playerSquare.x == right.x && playerSquare.y == right.y) {
            RemoveRight(gridPosition);
            return;
        }
        //otherwise remove randomly for one of the adjacent rooms that is occupied
        List<int> choices = new List<int>();
        if (IsOccupied(up)) {
            choices.Add(0);
        }
        if (IsOccupied(down)) {
            choices.Add(1);
        }
        if (IsOccupied(left)) {
            choices.Add(2);
        }
        if (IsOccupied(right)) {
            choices.Add(3);
        }
        if (choices.Count == 0) {
            return;
        }
        int whichRoom = choices[UnityEngine.Random.Range(0, choices.Count)];
        if (whichRoom == 0) {
            RemoveUp(gridPosition);
            return;
        }
        if (whichRoom == 1) {
            RemoveDown(gridPosition);
            return;
        }
        if (whichRoom == 2) {
            RemoveLeft(gridPosition);
            return;
        }
        if (whichRoom == 3) {
            RemoveRight(gridPosition);
            return;
        }
    }


    //Approach: get x and y of player's current square, use that as the base for checking if occupied
    //in increasingly larger regions of squares around the base square, check to see if the square is occupied.
    //keep a running minimum of the distances between the base square and those squares that are unoccupied.
    //If there are any unoccupied rooms at the end of this layer, return the minimum distance one.
    private Vector2Int ClosestUnoccupiedGridSquare(Vector3 position) {
        //get x and y of player's current grid square, use that as the base.
        int xbase = (int)Mathf.Round(player.transform.position.x / cellSize);
        int ybase = (int)Mathf.Round(player.transform.position.y / cellSize);

        int width = 3;
        Vector2Int result = new Vector2Int(-width + xbase, -width + ybase);
        while (width < 100) {
            result = new Vector2Int(-width + xbase, -width + ybase);
            Vector3 pos = GridToWorldPosition(result, player.transform.position.z);
            float minimumDistance = (player.transform.position - pos).sqrMagnitude;
            bool foundInLayer = false;
            for (int x = -width; x <= width; x++) {
                for (int y = -width; y <= width; y++) {
                    Vector2Int xy = new Vector2Int(x + xbase, y + ybase);
                    if (!IsOccupied(xy)) {
                        foundInLayer = true;
                        pos = GridToWorldPosition(xy, player.transform.position.z);
                        float dist = (player.transform.position - pos).sqrMagnitude;
                        if (dist < minimumDistance) {
                            minimumDistance = dist;
                            result = xy;
                        }
                    }
                }
            }
            if (foundInLayer) {
                return result;
            }
            width += 7;
        }
        return result;
    }

    private Vector2Int PlayerGridSquare() {
        int xbase = (int)Mathf.Round(player.transform.position.x / cellSize);
        int ybase = (int)Mathf.Round(player.transform.position.y / cellSize);
        //print("player is in " + xbase + ", " + ybase);
        return new Vector2Int(xbase, ybase);
    }
    private Vector3 GridToWorldPosition(Vector2Int gridPosition, float z) {
        return new Vector3(cellSize * gridPosition.x, cellSize * gridPosition.y, z);
    }

    private bool IsOccupied(Vector2Int gridSpot) {
        return roomsInGrid.ContainsKey(gridSpot);
    }

    private void RemoveUp(Vector2Int xy) {
        Destroy(roomsInGrid[xy].transform.Find("WallUp").gameObject);
        Vector2Int up = new Vector2Int(xy.x, xy.y + 1);
        if (IsOccupied(up)) {
            Destroy(roomsInGrid[up].transform.Find("WallDown").gameObject);
        }
    }

    private void RemoveDown(Vector2Int xy) {
        Destroy(roomsInGrid[xy].transform.Find("WallDown").gameObject);
        Vector2Int down = new Vector2Int(xy.x, xy.y - 1);
        if (IsOccupied(down)) {
            Destroy(roomsInGrid[down].transform.Find("WallUp").gameObject);
        }
    }

    private void RemoveLeft(Vector2Int xy) {
        Destroy(roomsInGrid[xy].transform.Find("WallLeft").gameObject);
        Vector2Int left = new Vector2Int(xy.x - 1, xy.y);
        if (IsOccupied(left)) {
            Destroy(roomsInGrid[left].transform.Find("WallRight").gameObject);
        }
    }

    private void RemoveRight(Vector2Int xy) {
        Destroy(roomsInGrid[xy].transform.Find("WallRight").gameObject);
        Vector2Int right = new Vector2Int(xy.x + 1, xy.y);
        if (IsOccupied(right)) {
            Destroy(roomsInGrid[right].transform.Find("WallLeft").gameObject);
        }
    }

    private void RemoveWalls(Vector2Int xy) {
        RemoveUp(xy);
        RemoveDown(xy);
        RemoveLeft(xy);
        RemoveRight(xy);
    }


}
