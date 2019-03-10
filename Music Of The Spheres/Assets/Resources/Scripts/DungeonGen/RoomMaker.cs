using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomMaker : MonoBehaviour {
    public GameObject player;
    public GameObject NoteRoom;//room prefab
    public GameObject Wall;//wall prefab
    private Dictionary<int, GameObject> rooms;//all note rooms this script has generated, indexed by note
    private Dictionary<Tuple<int, int>, GameObject> roomsInGrid; //all note rooms this script has generated, indexed by position in grid

    public MidiEventHandler MEH;

    private IEnumerator coroutine;


    //grid settings
    private float cellSize;

    private bool putANote = false;
    private int baseNote;

    // Start is called before the first frame update
    void Start() {
        cellSize = Wall.transform.localScale.x - Wall.transform.localScale.y;
        rooms = new Dictionary<int, GameObject>();
        roomsInGrid = new Dictionary<Tuple<int, int>, GameObject>();
    }

    public void DestroyNoteRoom(int note) {
        //print("destroying room " + note);
        PowerOff(note);
        //rooms[note].GetComponent<SpriteRenderer>().enabled = false;
        rooms[note].GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
    }

    public void PowerOn(int note) {
        rooms[note].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        coroutine = MakeLessBright(0.5f / MEH.midiTimeRate, note);
        StartCoroutine(coroutine);
    }

    public void PowerOff(int note) {
        rooms[note].GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1f);
    }

    private IEnumerator MakeLessBright(float waitTime, int note) {
        yield return new WaitForSeconds(waitTime);
        rooms[note].GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
    }

    //create normal note room which responds to note at closest unoccupied grid square to the player
    public void CreateNoteRoom(int note) {
        Tuple<int, int> gridPosition = ClosestUnoccupiedGridSquare(player.transform.position);
        Vector3 worldPosition = GridToWorldPosition(gridPosition, player.transform.position.z);
        GameObject newRoom = Instantiate(NoteRoom, worldPosition, Quaternion.identity, gameObject.transform);
        newRoom.name = "Room " + note;
        rooms[note] = newRoom;
        roomsInGrid[gridPosition] = newRoom;
        int x = gridPosition.Item1;
        int y = gridPosition.Item2;
        //print(x + ", " + y);
        Tuple<int, int> up = new Tuple<int, int>(x, y + 1);
        Tuple<int, int> down = new Tuple<int, int>(x, y - 1);
        Tuple<int, int> left = new Tuple<int, int>(x - 1, y);
        Tuple<int, int> right = new Tuple<int, int>(x + 1, y);
        if (IsOccupied(up)) {
            RemoveUp(gridPosition);
        }
        if (IsOccupied(down)) {
            RemoveDown(gridPosition);
        }
        if (IsOccupied(left)) {
            RemoveLeft(gridPosition);
        }
        if (IsOccupied(right)) {
            RemoveRight(gridPosition);
        }
    }


    //Approach: get x and y of player's current square, use that as the base for checking if occupied
    //in increasingly larger regions of squares around the base square, check to see if the square is occupied.
    //keep a running minimum of the distances between the base square and those squares that are unoccupied.
    //If there are any unoccupied rooms at the end of this layer, return the minimum distance one.
    private Tuple<int, int> ClosestUnoccupiedGridSquare(Vector3 position) {
        //get x and y of player's current grid square, use that as the base.
        int xbase = (int)((player.transform.position.x + 0.5) / cellSize);
        int ybase = (int)((player.transform.position.y + 0.5) / cellSize);

        int width = 3;
        Tuple<int, int> result = new Tuple<int, int>(-width + xbase, -width + ybase);
        while (width < 100) {
            result = new Tuple<int, int>(-width + xbase, -width + ybase);
            Vector3 pos = GridToWorldPosition(result, player.transform.position.z);
            float minimumDistance = (player.transform.position - pos).sqrMagnitude;
            bool foundInLayer = false;
            for (int x = -width; x < width; x++) {
                for (int y = -width; y < width; y++) {
                    Tuple<int, int> xy = new Tuple<int, int>(x + xbase, y + ybase);
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

    private Tuple<int, int> PlayerGridSquare() {
        int xbase = (int)((player.transform.position.x + 0.5) / cellSize);
        int ybase = (int)((player.transform.position.y + 0.5) / cellSize);
        return new Tuple<int, int>(xbase, ybase);
    }
    private Vector3 GridToWorldPosition(Tuple<int, int> gridPosition, float z) {
        return new Vector3(cellSize * gridPosition.Item1, cellSize * gridPosition.Item2, z);
    }

    private bool IsOccupied(Tuple<int, int> gridSpot) {
        return roomsInGrid.ContainsKey(gridSpot);
    }

    private void RemoveUp(Tuple<int, int> xy) {
        Tuple<int, int> up = new Tuple<int, int>(xy.Item1, xy.Item2 + 1);
        Destroy(roomsInGrid[xy].transform.Find("WallUp").gameObject);
        Destroy(roomsInGrid[up].transform.Find("WallDown").gameObject);
    }

    private void RemoveDown(Tuple<int, int> xy) {
        Tuple<int, int> down = new Tuple<int, int>(xy.Item1, xy.Item2 - 1);
        Destroy(roomsInGrid[xy].transform.Find("WallDown").gameObject);
        Destroy(roomsInGrid[down].transform.Find("WallUp").gameObject);
    }

    private void RemoveLeft(Tuple<int, int> xy) {
        Tuple<int, int> left = new Tuple<int, int>(xy.Item1 - 1, xy.Item2);
        Destroy(roomsInGrid[xy].transform.Find("WallLeft").gameObject);
        Destroy(roomsInGrid[left].transform.Find("WallRight").gameObject);
    }

    private void RemoveRight(Tuple<int, int> xy) {
        Tuple<int, int> right = new Tuple<int, int>(xy.Item1 + 1, xy.Item2);
        Destroy(roomsInGrid[xy].transform.Find("WallRight").gameObject);
        Destroy(roomsInGrid[right].transform.Find("WallLeft").gameObject);
    }


}
