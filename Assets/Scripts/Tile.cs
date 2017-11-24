using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    public TextMesh displayText;

    public Material materialIdle;
    public Material materialActive;
    public Material materialAvailable;

    public int indexNumber;
    public int tilesPerRow;
    public int state = 0; //0 = inactive, 1 = movement, 2 = active;

    public GameObject tileUpper;
    public GameObject tileLower;
    public GameObject tileLeft;
    public GameObject tileRight;
    public GameObject tileUpperRight;
    public GameObject tileUpperLeft;
    public GameObject tileLowerRight;
    public GameObject tileLowerLeft;

    public bool isOccupied = false;

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
     * If a mouse is hovering over the tile, set material to active
     */
    void OnMouseOver()
    {
        GetComponent<Renderer>().material = materialActive;
    }

    /*
     * Update material
     */
    void OnMouseExit()
    {
        setMaterial();
    }

    /*
     * Set materials according to states
     */
    public void setMaterial()
    {
        if (state == 0)
            GetComponent<Renderer>().material = materialIdle;
        else if (state == 1)
            GetComponent<Renderer>().material = materialAvailable;
    }

    /*
     * Following setState... functions are recursive and do almost the same thing, except traverse in the direction from they came from.
     * This makes the state setting spread in a tree-like pattern.
     */
    public void setStateUp(int aP, int newState)
    {
        if (aP == 0 || state == newState)
            return;
        this.state = newState;
        Debug.Log("SETTING " + indexNumber + " with AP " + aP);
        setMaterial();

        int newAP = aP - 1;
        if (tileUpper)
            tileUpper.GetComponent<Tile>().setStateUp(newAP, newState);
        if (tileRight)
            tileRight.GetComponent<Tile>().setStateUp(newAP, newState);
        if (tileLeft)
            tileLeft.GetComponent<Tile>().setStateUp(newAP, newState);
    }

    public void setStateLower(int aP, int newState)
    {
        if (aP == 0 || state == newState)
            return;
        this.state = newState;
        Debug.Log("SETTING " + indexNumber + " with AP " + aP);
        setMaterial();

        int newAP = aP - 1;
        if (tileLower)
            tileLower.GetComponent<Tile>().setStateLower(newAP, newState);
        if (tileRight)
            tileRight.GetComponent<Tile>().setStateLower(newAP, newState);
        if (tileLeft)
            tileLeft.GetComponent<Tile>().setStateLower(newAP, newState);
    }

    public void setStateLeft(int aP, int newState)
    {
        if (aP == 0 || state == newState)
            return;
        this.state = newState;
        Debug.Log("SETTING " + indexNumber + " with AP " + aP);
        setMaterial();

        int newAP = aP - 1;
        if (tileUpper)
            tileUpper.GetComponent<Tile>().setStateLeft(newAP, newState);
        if (tileLower)
            tileLower.GetComponent<Tile>().setStateLeft(newAP, newState);
        if (tileLeft)
            tileLeft.GetComponent<Tile>().setStateLeft(newAP, newState);
    }

    public void setStateRight(int aP, int newState)
    {
        if (aP == 0 || state == newState)
            return;
        this.state = newState;
        Debug.Log("SETTING " + indexNumber + " with AP " + aP);
        setMaterial();

        int newAP = aP - 1;
        if (tileUpper)
            tileUpper.GetComponent<Tile>().setStateRight(newAP, newState);
        if (tileLower)
            tileLower.GetComponent<Tile>().setStateRight(newAP, newState);
        if (tileRight)
            tileRight.GetComponent<Tile>().setStateRight(newAP, newState);
    }

    /*
     * Initial setState function passed from GridGenerator
     */ 
    public void setState(int aP, int newState)
    {
        if (aP == 0 || state == newState)
            return;
        this.state = newState;
        Debug.Log("SETTING " + indexNumber + " with AP " + aP);
        setMaterial();

        int newAP = aP-1;
        if (tileUpper)
            tileUpper.GetComponent<Tile>().setStateUp(newAP, newState);
        if (tileLower)
            tileLower.GetComponent<Tile>().setStateLower(newAP, newState);
        if (tileRight)
            tileRight.GetComponent<Tile>().setStateRight(newAP, newState);
        if (tileLeft)
            tileLeft.GetComponent<Tile>().setStateLeft(newAP, newState);
    }


    // Update is called once per frame
    void Update()
    {
    }
}
