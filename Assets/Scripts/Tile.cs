using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    public TextMesh displayText;

    public Material materialIdle;
    public Material materialActive;
    public Material materialAvailable;
    public Material materialAttack;

    public int indexNumber;
    public int tilesPerRow;
    public int state = 0; //0 = inactive, 1 = movement, 2 = active;

    private GameObject tileUpper;
    private GameObject tileLower;
    private GameObject tileLeft;
    private GameObject tileRight;
    private GameObject tileUpperRight;
    private GameObject tileUpperLeft;
    private GameObject tileLowerRight;
    private GameObject tileLowerLeft;

    public bool isOccupied = false;
    public int remAP = 0;

    public List<GameObject> adjacentTiles = new List<GameObject>();

    // Use this for initialization
    void Start () {
        gameObject.name = "Tile " + indexNumber.ToString();
        //Set default material to idle
        GetComponent<Renderer>().material = materialIdle;
        //Link 8 different neighbouring tiles by first checking
        if (InBounds(GridGenerator.tileArray, indexNumber + tilesPerRow)) { tileUpper = GridGenerator.tileArray[indexNumber + tilesPerRow]; }
        if (InBounds(GridGenerator.tileArray, indexNumber - tilesPerRow)) { tileLower = GridGenerator.tileArray[indexNumber - tilesPerRow]; }
        if (InBounds(GridGenerator.tileArray, indexNumber + 1) && (indexNumber + 1) % tilesPerRow != 0) { tileRight = GridGenerator.tileArray[indexNumber + 1]; }
        if (InBounds(GridGenerator.tileArray, indexNumber - 1) && indexNumber % tilesPerRow != 0) { tileLeft = GridGenerator.tileArray[indexNumber - 1]; }

        if (InBounds(GridGenerator.tileArray, indexNumber + tilesPerRow + 1) && (indexNumber + tilesPerRow + 1) % tilesPerRow != 0) { tileUpperRight = GridGenerator.tileArray[indexNumber + tilesPerRow + 1]; }
        if (InBounds(GridGenerator.tileArray, indexNumber + tilesPerRow - 1) && indexNumber % tilesPerRow != 0) { tileUpperLeft = GridGenerator.tileArray[indexNumber + tilesPerRow - 1]; }
        if (InBounds(GridGenerator.tileArray, indexNumber - tilesPerRow + 1) && (indexNumber + 1) % tilesPerRow != 0) { tileLowerRight = GridGenerator.tileArray[indexNumber - tilesPerRow + 1]; }
        if (InBounds(GridGenerator.tileArray, indexNumber - tilesPerRow - 1) && indexNumber % tilesPerRow != 0) { tileLowerLeft = GridGenerator.tileArray[indexNumber - tilesPerRow - 1]; }
        //then assigning
        if (tileUpper) { adjacentTiles.Add(tileUpper); }
        if (tileLower) { adjacentTiles.Add(tileLower); }
        if (tileLeft) { adjacentTiles.Add(tileLeft); }
        if (tileRight) { adjacentTiles.Add(tileRight); }

        if (tileUpperLeft) { adjacentTiles.Add(tileUpperLeft); }
        if (tileUpperRight) { adjacentTiles.Add(tileUpperRight); }
        if (tileLowerLeft) { adjacentTiles.Add(tileLowerLeft); }
        if (tileLowerRight) { adjacentTiles.Add(tileLowerRight); }
        
    }
    /*
     * Check if the tile is in bounds (no larger or smaller than index) 
     */
    private bool InBounds(GameObject[] inputArray, int targetindexNumber)
    {
        if (targetindexNumber < 0 || targetindexNumber >= inputArray.Length)
        {
            return false;
        }
        else
        {
            return true;
        }
        
    }

    /*
     * Set materials according to states
     */
    void SetMaterial()
    {
        switch (state)
        {
            case 0:
                GetComponent<Renderer>().material = materialIdle;
                break;
            case 1:
                GetComponent<Renderer>().material = materialAvailable;
                break;
            case 2:
                GetComponent<Renderer>().material = materialAttack;
                break;
        }

    }

    /*
     * Initial setState function passed from GridGenerator
     */ 
    public void SetState(int range, int newState, int caseSwitch)
    {
        if (range == 0 || state == newState)
            return;
        this.state = newState;
        //Debug.Log("SETTING " + indexNumber + " with range " + range + " at state " + newState);
        switch (newState)
        {
            case 0:
                remAP = 0;
                break;
            case 1:
                remAP = range - 1;
                break;
            case 2:
                remAP = range - 1;
                break;
        }

        if (tileLower && caseSwitch != 1)
        {
            if (caseSwitch == 0)
                tileLower.GetComponent<Tile>().SetState(remAP, newState, 2);
            else tileLower.GetComponent<Tile>().SetState(remAP, newState, caseSwitch);
        }
        if (tileUpper && caseSwitch != 2)
        {
            if (caseSwitch == 0)
                tileUpper.GetComponent<Tile>().SetState(remAP, newState, 1);
            else tileUpper.GetComponent<Tile>().SetState(remAP, newState, caseSwitch);

        }
        if (tileRight && caseSwitch != 3)
        {
            if (caseSwitch == 0)
                tileRight.GetComponent<Tile>().SetState(remAP, newState, 4);
            else tileRight.GetComponent<Tile>().SetState(remAP, newState, caseSwitch);
        }
        if (tileLeft && caseSwitch != 4)
        {
            if (caseSwitch == 0)
                tileLeft.GetComponent<Tile>().SetState(remAP, newState, 3);
            else tileLeft.GetComponent<Tile>().SetState(remAP, newState, caseSwitch);
        }

        if (isOccupied && newState == 1)
            state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        SetMaterial();

        if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Tile")
        {
            hit.collider.gameObject.GetComponent<Renderer>().material = materialActive;
        }
    }
}
