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
    public GameObject islandPrefab;
    public Text uiText;
    public Text buttonText;

    public int numberOfTiles = 100;
    public int tilesPerRow = 10;
    public float distanceBetweenTiles = 1.2f;

    public int numberOfPlayers = 2;
    public int numberOfShips = 2;
    public int iniActionPoints = 5;

    public int state = 1; // 0 = inactive, 1 = movement, 2 = attack;

    public static GameObject[] tileArray;
    public static GameObject[] playerArray;

    public GameObject currentShip;
    public GameObject currentTile;
    public int currentPlayer = -1;
    private int turnCount = 0;
    
    // Use this for initialization
    void Start()
    {
        CreateTiles();
        CreatePlayers();
        buttonText.text = "Start Game";
    }

    void CreateTiles()
    {
        //initialize tile array
        tileArray = new GameObject[numberOfTiles];
        //initialize offsets
        float xOffset = 0.0f;
        float zOffset = 0.0f;
        int islandSpawnPos = (numberOfTiles - 2*tilesPerRow)/2  + (tilesPerRow / 2) - 1;
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
            //create Island at center of map
            if (tilesCreated == islandSpawnPos)
            {
                var newIsland = Instantiate(islandPrefab, transform);
                newIsland.transform.position = new Vector3(newTile.transform.position.x + .5f, newTile.transform.position.y + 0.5f, newTile.transform.position.z + 1f);
                script.isOccupied = true;
                script.occuObject = newIsland;
                script.islandTile = true;
                newIsland.name = "floatIsland";
            }

            //insert into tileArray
            tileArray[tilesCreated] = newTile;
        }
    }

    void CreatePlayers()
    {
        playerArray = new GameObject[numberOfPlayers];
        for (int pN = 0; pN < numberOfPlayers; pN++)
        {
            var newPlayer = Instantiate(playerPrefab, transform);
            var playerScript = newPlayer.GetComponent<Players>();
            playerScript.tileArray = tileArray;
            playerScript.numberOfShips = numberOfShips;
            playerScript.iniActionPoints = iniActionPoints;
            playerScript.playerNumber = pN;
            playerArray[pN] = newPlayer;
        }
            currentShip = playerArray[currentPlayer+1].GetComponent<Players>().firstShip;
    }

    void UpdateText()
    {
        int nowP = currentPlayer + 1;
        if (turnCount == 0)
        {
            uiText.text = "Game Starting";
        }
        else
        { 
            uiText.text = "Player: " + nowP.ToString() + "\nTurn #" + turnCount.ToString() + "\nPhase " + state.ToString();
        }

    }

    /*
     * Function for updating Tile states. 0 = inactive, 1 = Selection Available.
     */
    void UpdateTiles(int range, int newState)
    {
        currentTile.GetComponent<Tile>().SetState(range+1, newState, 0);
    }

    /*
     * A brute force method to resetting tiles to 0 state.
     */
    void ClearTiles()
    {
        foreach (GameObject tiles in tileArray)
        {
            var tileScript = tiles.GetComponent<Tile>();
            if (tileScript.state != 3)
                tiles.GetComponent<Tile>().state = 0;
        }
    }

    /*
     * Function for moving ship to selected newTile.
     */
    void MoveShip(GameObject newTile)
    {
        currentShip.GetComponent<Ship>().tile.GetComponent<Tile>().isOccupied = false;
        currentShip.GetComponent<Ship>().tile.GetComponent<Tile>().occuObject = null;
        ClearTiles(); //set state=1 tiles to 0.
        currentShip.GetComponent<Ship>().tile = newTile; //updates currentShip tile to newTile
        currentShip.GetComponent<Ship>().curActionPoints = newTile.GetComponent<Tile>().remAP;
        currentTile = newTile; //updates currentTile to newTile
        newTile.GetComponent<Tile>().isOccupied = true;
        newTile.GetComponent<Tile>().occuObject = currentShip; //indicate new tile is occupied

    }

    /*
    * A recursive function that returns an array of indices of tiles were isOccupied is true within the specified range
    * will however mark every occupied square in range as a possible target
    */
    private List<int> FindTargets(int range, GameObject tile)
    {
        List<int> foundObjects = new List<int>();
        if (tile.GetComponent<Tile>().isOccupied && tile.GetComponent<Tile>().occuObject.tag == "Ship")
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
                foundObjects = foundObjects.Union(FindTargets(range - 1, adjacent)).ToList();
            }
        }

        return foundObjects;
    }

    /*
    * A method that recieves a list of indices and accesses the corresponding tiles and sets the material to Attack
    */
    private void SetTargets(List<int> indices)
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
    public void AttackButton()
    {
        if (currentShip.GetComponent<Ship>().curActionPoints > 0)
        {
            if (state == 1)
            {
                state = 2;
            }
            else
            {
                state = 1;
            }
        }
        UpdateText();
    }

    public void EndTurn()
    {
        buttonText.text = "Pass Turn";
        currentPlayer++;
        if (currentPlayer+1 > numberOfPlayers)
        {
            currentPlayer = 0;
        }
        UpdateText();
        foreach (GameObject player in playerArray)
        {
            //Debug.Log("Setting " + player + " with currentPlayer " + currentPlayer); 
            player.GetComponent<Players>().SetTurn(currentPlayer);
        }
        currentShip = null;
        turnCount++;
        ClearTiles();
    }

    // Update is called once per frame
    void Update()
    {
        //Ray shoots from camera POV
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetButtonDown("Fire1") && Physics.Raycast(ray, out hit))
        {
            GameObject hitGO = hit.collider.gameObject;
            if (hit.collider.tag == "Ship" && hitGO.GetComponent<Ship>().currentPlayerTurn == true && hitGO != currentShip && hitGO.GetComponent<Ship>().state != 3)
            {
                ClearTiles();
                currentShip = hitGO; //switch operating ship to new
                currentTile = currentShip.GetComponent<Ship>().tile; //update current tile
                iniActionPoints = currentShip.GetComponent<Ship>().iniActionPoints;
                UpdateTiles(iniActionPoints, 1); //update tiles for new ship
                //List<int> targets = FindTargets(currentShip.GetComponent<Ship>().atkRange, currentTile); //redundant here
                //SetTargets(targets); //redundant here
            }
            else if (hit.collider.tag == "Tile" && hitGO.GetComponent<Tile>().isOccupied && hitGO.GetComponent<Tile>().occuObject.tag == "Ship" &&
                     hitGO.GetComponent<Tile>().occuObject.GetComponent<Ship>().currentPlayerTurn == true && hitGO.GetComponent<Tile>().occuObject.GetComponent<Ship>().state != 3 &&
                     hitGO.GetComponent<Tile>().occuObject != currentShip)
            {
                ClearTiles();
                currentShip = hitGO.GetComponent<Tile>().occuObject; //switch operating ship to new
                currentTile = currentShip.GetComponent<Ship>().tile; //update current tile
                iniActionPoints = currentShip.GetComponent<Ship>().iniActionPoints;
                UpdateTiles(iniActionPoints, 1); //update tiles for new ship
            }

        }

        if (currentShip != null && currentShip.GetComponent<Ship>().state != 3)
        {
            currentTile = currentShip.GetComponent<Ship>().tile;
            iniActionPoints = currentShip.GetComponent<Ship>().iniActionPoints;
            List<int> targets = FindTargets(currentShip.GetComponent<Ship>().atkRange, currentTile);
            SetTargets(targets);
            currentShip.GetComponent<Ship>().stats.GetComponent<Renderer>().enabled = true;
        }
        if (state == 1)
        {
            if (currentShip != null)
            {
                //currentShip.GetComponent<Ship>().curActionPoints = iniActionPoints;
                UpdateTiles(currentShip.GetComponent<Ship>().curActionPoints, 1);
            }
            if (Input.GetButtonDown("Fire1") && Physics.Raycast(ray, out hit)) //On Mouse Click
            {
                GameObject hitGO = hit.collider.gameObject;
                if (currentShip != null)
                    currentTile = currentShip.GetComponent<Ship>().tile;
                if (hit.collider.tag == "Tile") //if ray hits and tag is a "tile"
                {
                    //hit.collider.gameObject now refers to the tile under the mouse cursor
                    if (hitGO.GetComponent<Tile>().state == 1)
                    {
                        MoveShip(hitGO); //move the ship if tile state is 1
                        state = 999;
                    }
                    else if (hitGO.GetComponent<Tile>().state == 3 && hitGO.GetComponent<Tile>().IsAdjacent(currentTile))
                    {
                        currentShip.GetComponent<Ship>().SetState(3);
                        ClearTiles();
                        currentShip = null;
                    }
                }
            }
        }
        // conly shoot at target if it's a ship not controlled by player and the targets tile's state is 2
        if (state == 2 && currentShip != null && !currentShip.GetComponent<Ship>().hasAttacked)
        {
            UpdateTiles(currentShip.GetComponent<Ship>().atkRange, 2);
            if (Input.GetButtonDown("Fire1") && Physics.Raycast(ray, out hit)) //On Mouse Click
            {
                GameObject hitGO = hit.collider.gameObject;
                if (hitGO.tag == "Ship" && hitGO.GetComponent<Ship>().currentPlayerTurn == false && hitGO.GetComponent<Ship>().tile.GetComponent<Tile>().state == 2)
                {
                    hitGO.GetComponent<Ship>().GetDamaged(currentShip.GetComponent<Ship>().damage);
                    state = 1;
                }
                if (hitGO.tag == "Tile" && hitGO.GetComponent<Tile>().isOccupied == true && hitGO.GetComponent<Tile>().state == 2)
                {
                    hitGO.GetComponent<Tile>().occuObject.GetComponent<Ship>().GetDamaged(currentShip.GetComponent<Ship>().damage);
                    state = 1;
                }
                currentShip.GetComponent<Ship>().hasAttacked = true;

            }
        }

        /*
         * Update during movement
         */
        if (state == 999)
        {
            Vector3 newPosition = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + 0.35f, currentTile.transform.position.z);
            if (currentShip.transform.position != newPosition)
                currentShip.transform.position = Vector3.MoveTowards(currentShip.transform.position, newPosition, 5f * Time.deltaTime);
            else
            {
                List<int> targets = FindTargets(currentShip.GetComponent<Ship>().atkRange, currentTile);
                SetTargets(targets);
                state = 1;
            }
        }
        UpdateText();
    }
}
