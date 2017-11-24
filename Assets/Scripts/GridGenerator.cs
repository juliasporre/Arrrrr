using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour {
    public GameObject tilePrefab;
    public GameObject shipPrefab;

    public int numberOfTiles = 100;
    public int tilesPerRow = 10;
    public float distanceBetweenTiles = 1.2f;

    public int numberOfShips = 1;

    public int state = 1; // 0 = inactive, 1 = movement, 2 = attack;

    public static GameObject[] tileArray;
    public static GameObject[] shipArray;

    public GameObject currentShip;
    public GameObject currentTile;
    public int actionPoints = 5;

    // Use this for initialization
    void Start ()
    {
        CreateTiles();
        CreateShips();
	}

    void CreateTiles()
    {
        //initialize tile array
        tileArray = new GameObject[numberOfTiles];
        //initialize offsets
        float xOffset = 0.0f;
        float zOffset = 0.0f;
        //create array of tiles
        for (int tilesCreated = 0; tilesCreated < numberOfTiles; tilesCreated++)
        {
            xOffset += distanceBetweenTiles;
            //last tile on row will have xOffset reset, zOffset increased
            if (tilesCreated % tilesPerRow == 0)
            {
                zOffset += distanceBetweenTiles;
                xOffset = 0;
                //special case for first tile
                if (tilesCreated == 0)
                    zOffset = 0;
            }
            //instantiate tile
            var newTile = Instantiate(tilePrefab, transform);
            newTile.transform.position = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset);
            var script = newTile.GetComponent<Tile>();
            //assign variables to tile
            script.indexNumber = tilesCreated;
            script.tilesPerRow = tilesPerRow;
            //insert into tileArray
            tileArray[tilesCreated] = newTile;
        }
    }

    void CreateShips()
    {
        //create ship array for potential multiple ships
        shipArray = new GameObject[numberOfShips];
        int randTileIndex = Random.Range(0, tileArray.Length);
        float yOffset = 0.35f;
        for (int shipsCreated = 0; shipsCreated < numberOfShips; shipsCreated++)
        {
            //instantiate new ship
            var newShip = Instantiate(shipPrefab, transform);
            //assign variables to new ship
            currentTile = tileArray[randTileIndex];
            currentTile.GetComponent<Tile>().isOccupied = true;
            newShip.GetComponent<Ship>().tile = currentTile;
            //assign position to ship
            newShip.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + yOffset, currentTile.transform.position.z);
            currentShip = newShip;
            newShip.GetComponent<Ship>().actionPoints = actionPoints;
            //insert into shipArray
            shipArray[shipsCreated] = newShip;
        }
    }
    /*
     * Function for updating Tile states. 0 = inactive, 1 = Selection Available.
     * Currently it serves for both setting and clearing the states, it should be changed to two different functions.
     */
    void updateTiles(int newState)
    {
        currentTile.GetComponent<Tile>().setState(actionPoints, newState);
    }
    /*
     * Function for moving ship to selected newTile.
     */
    void moveShip(GameObject newTile)
    {
        currentShip.GetComponent<Ship>().tile.GetComponent<Tile>().isOccupied = false;
        //set 1 tiles to 0.
        updateTiles(0);
        currentShip.GetComponent<Ship>().tile = newTile;
        currentTile = newTile;
        newTile.GetComponent<Tile>().isOccupied = true;
        currentShip.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + 0.35f, currentTile.transform.position.z);
    }

    // Update is called once per frame
    void Update () {
        //Ray shoots from camera POV
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (state == 1)
        {
            currentTile = currentShip.GetComponent<Ship>().tile;
            actionPoints = currentShip.GetComponent<Ship>().actionPoints;
            updateTiles(1);
            if (Input.GetButtonDown("Fire1")) //On Mouse Click
            {
                if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Tile") //if ray hits and tag is a "tile"
                {
                        //hit.collider.gameObject now refers to the tile under the mouse cursor
                        if (hit.collider.gameObject.GetComponent<Tile>().state == 1)
                            moveShip(hit.collider.gameObject); //move the ship if tile state is 1
                }
            }
        }
	}
}
