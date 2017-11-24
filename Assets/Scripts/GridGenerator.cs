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
        tileArray = new GameObject[numberOfTiles];

        float xOffset = 0.0f;
        float zOffset = 0.0f;

        for (int tilesCreated = 0; tilesCreated < numberOfTiles; tilesCreated++)
        {
            xOffset += distanceBetweenTiles;

            if (tilesCreated % tilesPerRow == 0)
            {
                zOffset += distanceBetweenTiles;
                xOffset = 0;
                if (tilesCreated == 0)
                    zOffset = 0;
            }

            var newTile = Instantiate(tilePrefab, transform);
            newTile.transform.position = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z + zOffset);
            var script = newTile.GetComponent<Tile>();
            script.indexNumber = tilesCreated;
            script.tilesPerRow = tilesPerRow;
            
            tileArray[tilesCreated] = newTile;
        }
    }

    void CreateShips()
    {
        shipArray = new GameObject[numberOfShips];
        //int randTileIndex = Random.Range(0, tileArray.Length);
        float yOffset = 0.35f;
        for (int shipsCreated = 0; shipsCreated < numberOfShips; shipsCreated++)
        {
            var newShip = Instantiate(shipPrefab, transform);
            currentTile = tileArray[13];
            currentTile.GetComponent<Tile>().isOccupied = true;
            newShip.GetComponent<Ship>().tile = currentTile;
            newShip.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + yOffset, currentTile.transform.position.z);
            currentShip = newShip;
            newShip.GetComponent<Ship>().actionPoints = actionPoints;

            shipArray[shipsCreated] = newShip;
        }
    }

    void updateTiles(int newState)
    {
        currentTile.GetComponent<Tile>().setState(actionPoints, newState);
    }

    void moveShip(GameObject newTile)
    {
        currentShip.GetComponent<Ship>().tile.GetComponent<Tile>().isOccupied = false;
        updateTiles(0);
        currentShip.GetComponent<Ship>().tile = newTile;
        currentTile = newTile;
        newTile.GetComponent<Tile>().isOccupied = true;
        currentShip.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + 0.35f, currentTile.transform.position.z);
    }

    // Update is called once per frame
    void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (state == 1)
        {
            currentTile = currentShip.GetComponent<Ship>().tile;
            actionPoints = currentShip.GetComponent<Ship>().actionPoints;
            updateTiles(1);
            if (Input.GetButtonDown("Fire1"))
            {
                if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Tile")
                {
                        //hit.collider.gameObject now refers to the tile under the mouse cursor
                        if (hit.collider.gameObject.GetComponent<Tile>().state == 1)
                            moveShip(hit.collider.gameObject);
                }
            }
        }
	}
}
