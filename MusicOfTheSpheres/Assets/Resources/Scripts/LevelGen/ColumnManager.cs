using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnManager : MonoBehaviour {
    public GameObject player;
    public GameObject Column;//column prefab
    public GameObject Wall;//wall prefab
    public GameObject GenericMachine;//test machine prefab
    private Dictionary<int, GameObject> columns;//all note columns this script has generated, indexed by note
    private Dictionary<Vector2Int, GameObject> columnsInGrid; //all note columns this script has generated, indexed by position in grid
    public bool playNotes;

    public MidiEventHandler MEH;

    //grid settings
    private float cellSize;

    private int baseNote;
    // Start is called before the first frame update
    void Start() {
        cellSize = Column.transform.Find("Column").lossyScale.x;
        columns = new Dictionary<int, GameObject>();
        columnsInGrid = new Dictionary<Vector2Int, GameObject>();
    }

    private void Update() {
        //TODO: find the column the player is standing on
        //TODO: make the camera target that column
    }

    public void DestroyColumn(int note) {
        print("destroying column " + note);
        PowerOff(note);
        columns[note].transform.Find("Void").gameObject.SetActive(true);
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
        float degree = (note - 32) / 81f;
        //TODO: change object's color in the 3D way
        //newColumn.transform.Find("Floor").GetComponent<SpriteRenderer>().color = new Color(degree, 0f, (1f - degree) * 0.3f, 1f);
        //figure out which wall to remove from this column
        //if the player is in one of the adjacent squares, remove only the wall for that one
        int x = gridPosition.x;
        int y = gridPosition.y;
        //print(x + ", " + y);
        Vector2Int up = new Vector2Int(x, y + 1);
        Vector2Int down = new Vector2Int(x, y - 1);
        Vector2Int left = new Vector2Int(x - 1, y);
        Vector2Int right = new Vector2Int(x + 1, y);
        Vector2Int playerSquare = PlayerGridSquare();
        //for each occupied adjacent column, if it's within the player's travel range, remove the wall
        int noteRange = 5;
        if (IsOccupied(up) && Mathf.Abs(columnsInGrid[up].GetComponent<Column>().note - note) <= noteRange) {
            RemoveUp(gridPosition);
        }
        if (IsOccupied(down) && Mathf.Abs(columnsInGrid[down].GetComponent<Column>().note - note) <= noteRange) {
            RemoveDown(gridPosition);
        }
        if (IsOccupied(left) && Mathf.Abs(columnsInGrid[left].GetComponent<Column>().note - note) <= noteRange) {
            RemoveLeft(gridPosition);
        }
        if (IsOccupied(right) && Mathf.Abs(columnsInGrid[right].GetComponent<Column>().note - note) <= noteRange) {
            RemoveRight(gridPosition);
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
        Vector2Int result = new Vector2Int(-width + xbase, -width + ybase);
        while (width < 100) {
            result = new Vector2Int(-width + xbase, -width + ybase);
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
            width += 7;
        }
        return result;
    }

    private Vector2Int PlayerGridSquare() {
        int xbase = (int)Mathf.Round(player.transform.position.x / cellSize);
        int ybase = (int)Mathf.Round(player.transform.position.y / cellSize);
        return new Vector2Int(xbase, ybase);
    }
    private Vector3 GridToWorldPosition(Vector2Int gridPosition, int note) {
        return new Vector3(cellSize * gridPosition.x, note - 64, cellSize * gridPosition.y);
    }

    private bool IsOccupied(Vector2Int gridSpot) {
        return columnsInGrid.ContainsKey(gridSpot);
    }

    private void RemoveUp(Vector2Int xy) {
        Destroy(columnsInGrid[xy].transform.Find("WallUp").gameObject);
        Vector2Int up = new Vector2Int(xy.x, xy.y + 1);
        if (IsOccupied(up)) {
            Destroy(columnsInGrid[up].transform.Find("WallDown").gameObject);
        }
    }

    private void RemoveDown(Vector2Int xy) {
        Destroy(columnsInGrid[xy].transform.Find("WallDown").gameObject);
        Vector2Int down = new Vector2Int(xy.x, xy.y - 1);
        if (IsOccupied(down)) {
            Destroy(columnsInGrid[down].transform.Find("WallUp").gameObject);
        }
    }

    private void RemoveLeft(Vector2Int xy) {
        Destroy(columnsInGrid[xy].transform.Find("WallLeft").gameObject);
        Vector2Int left = new Vector2Int(xy.x - 1, xy.y);
        if (IsOccupied(left)) {
            Destroy(columnsInGrid[left].transform.Find("WallRight").gameObject);
        }
    }

    private void RemoveRight(Vector2Int xy) {
        Destroy(columnsInGrid[xy].transform.Find("WallRight").gameObject);
        Vector2Int right = new Vector2Int(xy.x + 1, xy.y);
        if (IsOccupied(right)) {
            Destroy(columnsInGrid[right].transform.Find("WallLeft").gameObject);
        }
    }

    private void RemoveWalls(Vector2Int xy) {
        RemoveUp(xy);
        RemoveDown(xy);
        RemoveLeft(xy);
        RemoveRight(xy);
    }
}
