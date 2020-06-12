using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnManager : MonoBehaviour {
    public GameObject player;
    public GameObject Column;//column prefab
    public GameObject Stair;//stair prefab
    public GameObject GenericMachine;//test machine prefab
    private Dictionary<int, GameObject> columns;//all note columns this script has generated, indexed by note
    public Dictionary<Vector2Int, GameObject> columnsInGrid; //all note columns this script has generated, indexed by position in grid
    public bool playNotes;

    public MidiEventHandler MEH;

    //grid settings
    private float cellSize;

    // Start is called before the first frame update
    void Start() {
        cellSize = Column.transform.Find("Platform").lossyScale.x;
        columns = new Dictionary<int, GameObject>();
        columnsInGrid = new Dictionary<Vector2Int, GameObject>();
    }

    private void Update() {
        Transform target = player.transform;
        if (columnsInGrid.ContainsKey(PlayerGridSquare()) && columnsInGrid[PlayerGridSquare()]) {
            target = columnsInGrid[PlayerGridSquare()].transform;
        }
        Camera.main.GetComponent<CameraController>().Target = target;
    }

    public void DestroyColumn(int note) {
        print("destroying column " + note);
        columnsInGrid.Remove(columns[note].GetComponent<Column>().pos);
        Destroy(columns[note]);
        columns.Remove(note);
    }

    public void PowerOn(int note) {
        columns[note].GetComponent<Column>().PowerOn();
    }

    public void PowerOff(int note) {
        columns[note].GetComponent<Column>().PowerOff();
    }

    //create normal note column which responds to note at closest unoccupied grid square to the player
    public void CreateColumn(int note) {
        Vector2Int gridPosition = ClosestUnoccupiedGridSquare(player.transform.position);
        Vector3 worldPosition = GridToWorldPosition(gridPosition, note);
        GameObject newColumn = Instantiate(Column, worldPosition, Quaternion.identity, gameObject.transform);
        newColumn.name = "Column " + note;
        columns[note] = newColumn;
        newColumn.GetComponent<Column>().note = note;
        columnsInGrid[gridPosition] = newColumn;
        newColumn.GetComponent<Column>().pos = gridPosition;
        newColumn.GetComponent<Column>().playNotes = playNotes;
        newColumn.transform.Find("Platform").GetComponent<Platform>().height = note;

        //TODO: use this machine spawning logic once machine prefab is done
        //for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++) {
        //    GameObject machine = Instantiate(
        //        GenericMachine,
        //        newColumn.transform.position +
        //        new Vector3(UnityEngine.Random.Range(-cellSize / 4, cellSize / 4),
        //                    newColumn.transform.position.y,
        //                    UnityEngine.Random.Range(-cellSize / 4, cellSize / 4)),
        //        Quaternion.identity,
        //        newColumn.transform);
        //    newColumn.transform.Find("PowerSource").GetComponent<PowerSource>().machines.Add(machine.GetComponent<Machine>());
        //}

        //figure out which walls to remove from this column
        //if the player is in one of the adjacent squares, remove only the wall for that one
        int x = gridPosition.x;
        int y = gridPosition.y;
        List<Vector2Int> neighbors = new List<Vector2Int> {
            new Vector2Int(x, y + 1),
            new Vector2Int(x, y - 1),
            new Vector2Int(x - 1, y),
            new Vector2Int(x + 1, y),
        };
        foreach (Vector2Int neighbor in neighbors) {
            if (IsOccupied(neighbor) && Mathf.Abs(columnsInGrid[neighbor].GetComponent<Column>().note - note) <= player.GetComponent<PlayerController>().noteRange) {
                print("connecting " + newColumn.name + " and " + columnsInGrid[neighbor].name);
                ConnectColumns(gridPosition, neighbor);
                ConnectColumns(neighbor, gridPosition);
            }
        }
    }


    //Approach: get x and y of player's current square, use that as the base for checking if occupied
    //in increasingly larger regions of squares around the base square, check to see if the square is occupied.
    //keep a running minimum of the distances between the base square and those squares that are unoccupied.
    //If there are any unoccupied columns at the end of this layer, return the minimum distance one.
    private Vector2Int ClosestUnoccupiedGridSquare(Vector3 position) {
        //get x and y of player's current grid square, use that as the base.
        Vector2Int playerSquare = PlayerGridSquare();
        int xbase = playerSquare.x;
        int ybase = playerSquare.y;

        int width = 3;
        while (true) {
            Vector2Int result = new Vector2Int(-width + xbase, -width + ybase);
            Vector3 pos = GridToWorldPosition(result, 0);
            float minimumDistance = (player.transform.position - pos).sqrMagnitude;
            bool foundInLayer = false;
            for (int x = -width; x <= width; x++) {
                for (int y = -width; y <= width; y++) {
                    Vector2Int xy = new Vector2Int(x + xbase, y + ybase);
                    if (!IsOccupied(xy)) {
                        foundInLayer = true;
                        pos = GridToWorldPosition(xy, 0);
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
            width *= 2;
        }
    }

    public Vector2Int PlayerGridSquare() {
        int xbase = (int)Mathf.Round(player.transform.position.x / cellSize);
        int ybase = (int)Mathf.Round(player.transform.position.z / cellSize);
        return new Vector2Int(xbase, ybase);
    }
    private Vector3 GridToWorldPosition(Vector2Int gridPosition, int note) {
        return new Vector3(cellSize * gridPosition.x, note - 64, cellSize * gridPosition.y);
    }

    private bool IsOccupied(Vector2Int gridSpot) {
        return columnsInGrid.ContainsKey(gridSpot) && columnsInGrid[gridSpot];
    }

    private void ConnectColumns(Vector2Int p1, Vector2Int p2) {
        GameObject c1 = columnsInGrid[p1];
        GameObject c2 = columnsInGrid[p2];
        int noteDiff = c2.GetComponent<Column>().note - c1.GetComponent<Column>().note;
        GameObject lowerColumn = noteDiff > 0 ? c1 : c2;
        GameObject upperColumn = noteDiff > 0 ? c2 : c1;

        string lowerWallToRemove = "WallUp";
        string upperWallToRemove = "WallDown";

        float platformRadius = lowerColumn.transform.Find("Platform").localScale.z / 2;
        float platformHeight = lowerColumn.transform.Find("Platform").localScale.y;
        float stairWidth = Stair.transform.localScale.z;
        float stairHeight = Stair.transform.localScale.y;
        float offsetDistance = platformRadius - stairWidth / 2;
        Vector3 stairPos = lowerColumn.transform.Find("Platform").position + new Vector3(0, platformHeight / 2 + stairHeight / 2, 0);
        Quaternion stairOrientation = Quaternion.identity;
        Vector3 stairOffset = Vector3.up * stairHeight;
        int numStairs = noteDiff - 1;

        Vector2Int up = new Vector2Int(0, 1);
        Vector2Int down = new Vector2Int(0, -1);
        Vector2Int left = new Vector2Int(-1, 0);
        Vector2Int right = new Vector2Int(1, 0);
        //set things dependent on direction
        Vector2Int diff = upperColumn.GetComponent<Column>().pos - lowerColumn.GetComponent<Column>().pos;
        if (diff == up) {
            stairOrientation = Quaternion.identity;
            stairPos += new Vector3(0, 0, offsetDistance);
            stairOffset += new Vector3(0, 0, -stairWidth);
            lowerWallToRemove = "WallUp";
            upperWallToRemove = "WallDown";
        } else if (diff == down) {
            stairOrientation = Quaternion.identity;
            stairPos += new Vector3(0, 0, -offsetDistance);
            stairOffset += new Vector3(0, 0, stairWidth);
            lowerWallToRemove = "WallDown";
            upperWallToRemove = "WallUp";
        } else if (diff == left) {
            stairOrientation = Quaternion.Euler(0, 90, 0);
            stairPos += new Vector3(-offsetDistance, 0, 0);
            stairOffset += new Vector3(stairWidth, 0, 0);
            lowerWallToRemove = "WallLeft";
            upperWallToRemove = "WallRight";
        } else if (diff == right) {
            stairOrientation = Quaternion.Euler(0, 90, 0);
            stairPos += new Vector3(offsetDistance, 0, 0);
            stairOffset += new Vector3(-stairWidth, 0, 0);
            lowerWallToRemove = "WallRight";
            upperWallToRemove = "WallLeft";
        } else {
            print("Cannot connect columns " + c1 + " and " + c2);
            return;
        }
        lowerColumn.transform.Find(lowerWallToRemove).gameObject.SetActive(false);
        upperColumn.transform.Find(upperWallToRemove).gameObject.SetActive(false);
        PlaceStaircase(lowerColumn.transform.Find("Stairs"), stairPos, lowerColumn.GetComponent<Column>().note, stairOrientation, stairOffset, numStairs);
    }
    private void PlaceStaircase(Transform parent, Vector3 basePosition, int baseHeight, Quaternion orientation, Vector3 offset, int numStairs) {
        if (numStairs < 1) {
            return;
        }
        for (int i = 0; i < numStairs; i++) {
            Vector3 spawnPos = basePosition + new Vector3(offset.x * (numStairs - 1 - i), offset.y * i, offset.z * (numStairs - 1 - i));
            GameObject step = Instantiate(Stair, spawnPos, orientation, parent);
            int height = baseHeight + i + 1;
            step.GetComponent<Platform>().height = height;
            step.name = "Stair " + height;
        }
        //build stairwell
        PlaceStaircase(parent, basePosition, baseHeight, orientation, offset, numStairs - 1);
    }
}
