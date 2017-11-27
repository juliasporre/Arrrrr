using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
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
    public int iniActionPoints = 5;

    // Use this for initialization
    void Start()
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
        float yOffset = 0.35f;
        for (int shipsCreated = 0; shipsCreated < numberOfShips; shipsCreated++)
        {
            int randTileIndex = Random.Range(0, tileArray.Length);
            //instantiate new ship
            var newShip = Instantiate(shipPrefab, transform);
            //assign variables to new ship
            currentTile = tileArray[randTileIndex];
            currentTile.GetComponent<Tile>().isOccupied = true;
            newShip.GetComponent<Ship>().tile = currentTile;
            //assign position to ship
            newShip.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + yOffset, currentTile.transform.position.z);
            currentShip = newShip;
            newShip.GetComponent<Ship>().iniActionPoints = iniActionPoints;
            newShip.GetComponent<Ship>().indexNumber = shipsCreated;
            //insert into shipArray
            shipArray[shipsCreated] = newShip;
        }
    }
    /*
     * Function for updating Tile states. 0 = inactive, 1 = Selection Available.
     */
    void UpdateTiles(int range, int newState)
    {
        currentTile.GetComponent<Tile>().SetState(range, newState, 0);
    }

    /*
     * A brute force method to resetting tiles to 0 state.
     */ 
    void ClearTiles()
    {
        foreach (GameObject tiles in tileArray)
        {
            tiles.GetComponent<Tile>().state = 0;
        }
    }

    /*
     * Function for moving ship to selected newTile.
     */
    void MoveShip(GameObject newTile)
    {
        currentShip.GetComponent<Ship>().tile.GetComponent<Tile>().isOccupied = false;
        ClearTiles(); //set state=1 tiles to 0.
        currentShip.GetComponent<Ship>().tile = newTile; //updates currentShip tile to newTile
        currentShip.GetComponent<Ship>().curActionPoints = newTile.GetComponent<Tile>().remAP;
        currentTile = newTile; //updates currentTile to newTile
        newTile.GetComponent<Tile>().isOccupied = true; //indicate new tile is occupied

    }

    public void PassTurn()
    {
        state = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Ray shoots from camera POV
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        currentTile = currentShip.GetComponent<Ship>().tile;
        iniActionPoints = currentShip.GetComponent<Ship>().iniActionPoints;

        ClearTiles();

        if (state == 1)
        {
            currentShip.GetComponent<Ship>().curActionPoints = iniActionPoints;
            UpdateTiles(iniActionPoints, 1);
            if (Input.GetButtonDown("Fire1")) //On Mouse Click
            {
                if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Tile") //if ray hits and tag is a "tile"
                {
                    //hit.collider.gameObject now refers to the tile under the mouse cursor
                    if (hit.collider.gameObject.GetComponent<Tile>().state == 1)
                    {
                        MoveShip(hit.collider.gameObject); //move the ship if tile state is 1
                        state = 999;
                    }
                }
                else if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Ship")
                {
                    ClearTiles();
                    currentShip = hit.collider.gameObject; //switch operating ship to new
                    currentTile = currentShip.GetComponent<Ship>().tile; //update current tile
                    iniActionPoints = currentShip.GetComponent<Ship>().iniActionPoints;
                    UpdateTiles(iniActionPoints, 1); //update tiles for new ship
                }
            }
        }
        if (state == 2)
        {
            UpdateTiles(currentShip.GetComponent<Ship>().atkRange, 2);
            if (Input.GetButtonDown("Fire1")) //On Mouse Click
            {
                if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Ship") //if ray hits and tag is a "ship"
                {
                    //hit.collider.gameObject now refers to the tile under the mouse cursor
                    hit.collider.gameObject.GetComponent<Ship>().GetDamaged(currentShip.GetComponent<Ship>().damage);
                    state = 1;
                }

            }
        }

        if (state == 999)
        {

            Vector3 newPosition = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + 0.35f, currentTile.transform.position.z);
            if (currentShip.transform.position != newPosition)
                currentShip.transform.position = Vector3.MoveTowards(currentShip.transform.position, newPosition, 5f * Time.deltaTime);
            else
            {
                if (currentShip.GetComponent<Ship>().curActionPoints > 0)
                    state = 2;
                else state = 1;
            }
        }
    }
}
