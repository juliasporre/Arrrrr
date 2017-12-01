using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GridGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject shipPrefab;
    public GameObject playerPrefab;
    public Text playerText;

    public int numberOfTiles = 100;
    public int tilesPerRow = 10;
    public float distanceBetweenTiles = 1.2f;

    public int numberOfPlayers = 2;
    public int numberOfShips = 2;
    public int iniActionPoints = 5;

    public int state = 1; // 0 = inactive, 1 = movement, 2 = attack;

    public static GameObject[] tileArray;
    public List<GameObject> shipArray;
    public static GameObject[] playerArray;

    public GameObject currentShip;
    public GameObject currentTile;
    public int currentPlayer = 0;

    // Use this for initialization
    void Start()
    {
        CreateTiles();
        //CreateShips();
        CreatePlayers();
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

    void CreatePlayers()
    {

        playerArray = new GameObject[numberOfPlayers];
        for (int playerNumber = 0; playerNumber < numberOfPlayers; playerNumber++)
        {
            var newPlayer = Instantiate(playerPrefab, transform);
            newPlayer.GetComponent<Players>().tileArray = tileArray;
            newPlayer.GetComponent<Players>().numberOfShips = numberOfShips;
            newPlayer.GetComponent<Players>().iniActionPoints = iniActionPoints;
            newPlayer.GetComponent<Players>().playerNumber = playerNumber;
            playerArray[playerNumber] = newPlayer;
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

    /*
    * A recursive function that returns an array of indices of tiles were isOccupied is true within the specified range
    * will however mark every occupied square in range as a possible target
    */
    private List<int> findTargets(int range, GameObject tile)
    {
        List<int> foundObjects = new List<int>();
        if (tile.GetComponent<Tile>().isOccupied)
        {
            foundObjects.Add(tile.GetComponent<Tile>().indexNumber);
        }
        if (range == 0)
        {
            return foundObjects;
        }
        else
        {
            foreach (GameObject adjacent in tile.GetComponent<Tile>().getNESWtiles())
            {
                // make sure every index is unique
                foundObjects = foundObjects.Union(findTargets(range-1, adjacent)).ToList();
            }
        }

        return foundObjects;
    }

    /*
    * A method that recieves a list of indices and accesses the corresponding tiles and sets the material to Attack
    */
    private void setTargets(List<int> indices)
    {
        foreach (int index in indices)
        {
            GameObject tile = tileArray[index];
            tile.GetComponent<Tile>().state = 2;
            tile.GetComponent<Tile>().SetMaterial();
        }
    }

    /*
    * To be called we the users press the end turn button
    * iterates through the ship array and flips the value of currentPlayerTurn
    */
    public void endTurn()
    {
        ClearTiles();
        currentPlayer++;
        if (currentPlayer > numberOfPlayers-1)
        {
            currentPlayer = 0;
        }
        foreach (GameObject player in playerArray)
        {
            player.GetComponent<Players>().setTurn(currentPlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Ray shoots from camera POV
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (currentShip != null)
        {
            currentTile = currentShip.GetComponent<Ship>().tile;
            iniActionPoints = currentShip.GetComponent<Ship>().iniActionPoints;
            List<int> targets = findTargets(currentShip.GetComponent<Ship>().atkRange, currentTile);
            setTargets(targets);
            currentShip.GetComponent<Ship>().stats.GetComponent<Renderer>().enabled = true;
        }
        if (state == 1)
        {
            if (currentShip != null)
            {
                currentShip.GetComponent<Ship>().curActionPoints = iniActionPoints;
                UpdateTiles(iniActionPoints, 1);
            }
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
                else if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Ship" && hit.collider.gameObject.GetComponent<Ship>().currentPlayerTurn == true)
                {
                    ClearTiles();
                    currentShip = hit.collider.gameObject; //switch operating ship to new
                    currentTile = currentShip.GetComponent<Ship>().tile; //update current tile
                    iniActionPoints = currentShip.GetComponent<Ship>().iniActionPoints;
                    UpdateTiles(iniActionPoints, 1); //update tiles for new ship
                    List<int> targets = findTargets(currentShip.GetComponent<Ship>().atkRange, currentTile);
                    setTargets(targets);
                }
                // conly shoot at target if it's a ship not controlled by player and the targets tile's state is 2
                else if (Physics.Raycast(ray, out hit) && hit.collider.tag == "Ship" && hit.collider.gameObject.GetComponent<Ship>().currentPlayerTurn == false && hit.collider.gameObject.GetComponent<Ship>().tile.GetComponent<Tile>().state == 2)
                {
                    hit.collider.gameObject.GetComponent<Ship>().GetDamaged(currentShip.GetComponent<Ship>().damage);
                }
            }
        }
        // not used
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
                List<int> targets = findTargets(currentShip.GetComponent<Ship>().atkRange, currentTile);
                setTargets(targets);
                /*if (currentShip.GetComponent<Ship>().curActionPoints > 0)
                    state = 2;
                else state = 1;*/
                state = 1;
            }
        }
    }
}
