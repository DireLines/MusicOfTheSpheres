﻿using System.Collections;
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
        Transform newColumnPlatform = newColumn.transform.Find("Platform");
        newColumnPlatform.GetComponent<Platform>().height = note;

        for (int i = 0; i < Random.Range(1, 5); i++) {
            GameObject machine = Instantiate(
                GenericMachine,
                newColumnPlatform.position +
                new Vector3(Random.Range(-cellSize / 4, cellSize / 4),
                            newColumnPlatform.localScale.y / 2 + GenericMachine.transform.localScale.y / 2,
                            Random.Range(-cellSize / 4, cellSize / 4)),
                Quaternion.identity,
                newColumn.transform);
            newColumn.GetComponent<Column>().machines.Add(machine.GetComponent<Machine>());
        }

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
                ConnectColumns(gridPosition, neighbor);
                //TODO: why do I need this
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
    private void DeactivateWall(Transform wall, int noteDiff) {
        if (Mathf.Abs(noteDiff) <= 1) {
            wall.gameObject.SetActive(false);
        } else {
            wall.Find("Middle").gameObject.SetActive(false);
        }
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
        DeactivateWall(lowerColumn.transform.Find(lowerWallToRemove), noteDiff);
        DeactivateWall(upperColumn.transform.Find(upperWallToRemove), noteDiff);
        PlaceStaircase(lowerColumn.transform.Find("Stairs"), stairPos, lowerColumn.GetComponent<Column>().note, stairOrientation, stairOffset, numStairs);
    }
    private void PlaceStaircase(Transform parent, Vector3 basePosition, int baseHeight, Quaternion orientation, Vector3 offset, int numStairs) {
        if (numStairs < 1) {
            return;
        }

        for (int i = 0; i < numStairs; i++) {
            Vector3 startPos = basePosition + new Vector3(offset.x * (numStairs - 1 - i), 0f, offset.z * (numStairs - 1 - i));
            Vector3 endPos = startPos + Vector3.up * (i + 0.5f);
            GameObject step = Instantiate(Stair, startPos, orientation, parent);
            step.transform.localScale.Scale(Vector3.one * (numStairs - i)); //Set stair to the appropriate height
            step.name = "Stair " + (baseHeight + i + 1).ToString();
            StartCoroutine(PlaceStaircaseCR(step, startPos, endPos, i));
        }
        //build stairwell
        PlaceStaircase(parent, basePosition, baseHeight, orientation, offset, numStairs - 1);
    }

    private IEnumerator PlaceStaircaseCR(GameObject step, Vector3 startPos, Vector3 endPos, int delay) {
        float speedMultiplier = 2f;
        yield return new WaitForSeconds(delay / speedMultiplier / 2f);
        float t = 0f;
        step.transform.position = startPos;
        while (t < 1f) {
            step.transform.position = Vector3.Lerp(startPos, endPos, t);
            t += speedMultiplier * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        step.transform.position = endPos;
    }
}
