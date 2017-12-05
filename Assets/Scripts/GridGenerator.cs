using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class GridGenerator : NetworkBehaviour
{
    [SyncVar]
    public bool testvAR = false;

    public GameObject tilePrefab;
    public GameObject shipPrefab;
    public GameObject playerPrefab;
    public GameObject islandPrefab;
    public Text uiText;
    public Text buttonText;
    public GameObject option1;
    public GameObject option2;

    public int numberOfTiles = 100;
    public int tilesPerRow = 10;
    public float distanceBetweenTiles = 1.2f;

    public int numberOfPlayers = 2;
    public int numberOfShips = 2;
    public int iniActionPoints = 5;
    public int fogOfWarRange = 7;

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
        //buttonText.text = "Start Game";
        option1.SetActive(false);
        option2.SetActive(false);
        transform.position *= transform.localScale.x; 
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
            xOffset += distanceBetweenTiles * gameObject.transform.localScale.x;
            //last tile on row will have xOffset reset, zOffset increased
            if (tilesCreated % tilesPerRow == 0)
            {
                zOffset += distanceBetweenTiles * gameObject.transform.localScale.z;
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
            if (tilesCreated == 0 || tilesCreated == numberOfTiles-1)
            {
                script.state = 4;
            }
                if (tilesCreated == islandSpawnPos)
            {
                var newIsland = Instantiate(islandPrefab, transform);
                newIsland.transform.position = new Vector3(newTile.transform.position.x + .5f*transform.localScale.x, newTile.transform.position.y + 0.5f, newTile.transform.position.z + 1f*transform.localScale.z);
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
            if (tileScript.state != 3 && tileScript.state != 4)
            { 
                tileScript.state = 0;
                tileScript.remAP = -1;
            }
        }
    }

    /*
     * Function for moving ship to selected newTile.
     */
    void MoveShip(GameObject newTile)
    {
        currentShip.GetComponent<Ship>().tile.GetComponent<Tile>().isOccupied = false;
        currentShip.GetComponent<Ship>().tile.GetComponent<Tile>().occuObject = null;
        
        currentShip.GetComponent<Ship>().tile = newTile; //updates currentShip tile to newTile
        currentShip.GetComponent<Ship>().curActionPoints = newTile.GetComponent<Tile>().remAP;
        currentTile = newTile; //updates currentTile to newTile
        newTile.GetComponent<Tile>().isOccupied = true;
        newTile.GetComponent<Tile>().occuObject = currentShip; //indicate new tile is occupied

        ClearTiles(); //set state=1 tiles to 0.

    }

    /*
    * A recursive function that returns an array of indices of tiles were isOccupied is true within the specified range
    * will however mark every occupied square in range as a possible target
    */
    private List<int> FindTargets(int range, GameObject tile)
    {
        List<int> foundObjects = new List<int>();

        if (currentTile != null)
        {
            if (tile.GetComponent<Tile>().isOccupied && tile.GetComponent<Tile>().occuObject.tag == "Ship" && !tile.GetComponent<Tile>().occuObject.GetComponent<Ship>().currentPlayerTurn)
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
     * Method that updates Fog of War between Update calls. 
     */
    private void UpdateFOW()
    {

        if (currentTile == null && currentPlayer != -1)
            currentTile = playerArray[currentPlayer].GetComponent<Players>().shipArray[0].GetComponent<Ship>().tile;
        List<int> cTL = new List<int>();
        foreach (GameObject ship in playerArray[currentPlayer].GetComponent<Players>().shipArray)
        {
            var shipT = ship.GetComponent<Ship>().tile;
            cTL.AddRange(FindTargets(fogOfWarRange, shipT));
        }
        foreach (GameObject player in playerArray)
        {
            var players = player.GetComponent<Players>();

            if (players.playerNumber != currentPlayer)
            {
                players.FogOfWar(cTL, currentPlayer); //Call FogOfWar method if player is not CurrentPlayer.
            }
        }
    }


    private bool CheckVictory()
    {
        List<bool> boolist = new List<bool>();
        int valCount = 0;
        foreach (GameObject player in playerArray)
        {
            var playerScript = player.GetComponent<Players>();
            boolist.Add(playerScript.CheckShips());
            if (playerScript.CheckShips())
            {
                valCount++;
            }
            if (playerScript.CheckTreasure())
            {
                state = 998;
                currentShip = null;
                ClearTiles();
                uiText.text = "Player " + (playerScript.playerNumber+1).ToString() + " wins!";
                return true;
            }
        }
        
        if (valCount == 1)
        {
            int winplayer = boolist.IndexOf(true) +1;
            state = 998;
            currentShip = null;
            ClearTiles();
            uiText.text = "Player " + winplayer.ToString() + " wins!";
            return true;
        }
        return false;
    }

    /*
    * To be called we the users press the end turn button
    * iterates through the ship array and flips the value of currentPlayerTurn
    */
            public void AttackButton()
    {
        if (currentShip && currentShip.GetComponent<Ship>().curActionPoints > 0)
        {
            if (state == 1)
            {
                state = 2;
                option1.SetActive(true);
                option2.SetActive(true);
            }
            else
            {
                state = 1;
                option1.SetActive(false);
                option2.SetActive(false);
                ClearTiles();
            }
        }
        UpdateText();
    }

    public void ChangeWeapons(int number)
    {
        currentShip.GetComponent<Ship>().ChangeWeapons(number);
        ClearTiles();
    }

    /*
     * Button for Passing turn / starting game.
     */
    public void EndTurn()
    {
        buttonText.text = "Pass Turn";
        currentPlayer++;
        if (CheckVictory()) //if victory conditions are met
            return; //finish game immediately
        if (currentPlayer+1 > numberOfPlayers)
        {
            currentPlayer = 0;
        }
        UpdateText();
        foreach (GameObject player in playerArray)
        {
            player.GetComponent<Players>().SetTurn(currentPlayer);
        }
        currentShip = null;
        currentTile = null;
        turnCount++;
        ClearTiles();
        UpdateFOW();
        state = 1;
    }

    // Update is called once per frame
    void Update()
    {

		/*if (!isServer)
		{
			return;
		}*/


        if (Input.GetKey(KeyCode.S))
            testvAR = true;


        if (testvAR)
            Debug.Log("NU SYNKAS VÅR VARIABEL DETTA ÄR JULIA");

        //Ray shoots from camera POV
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction);
        RaycastHit hit;

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
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    //Debug.Log("Button has press");
                    return;
                }
                if (hit.collider.tag == "Ship" && hitGO.GetComponent<Ship>().currentPlayerTurn == true && hitGO != currentShip && hitGO.GetComponent<Ship>().state != 3)
                {
                    ClearTiles();
                    currentShip = hitGO; //switch operating ship to new
                    currentTile = currentShip.GetComponent<Ship>().tile; //update current tile
                    UpdateTiles(currentShip.GetComponent<Ship>().curActionPoints, 1); //update tiles for new ship
                    return;
                }
                else if (hit.collider.tag == "Tile" && hitGO.GetComponent<Tile>().isOccupied && hitGO.GetComponent<Tile>().occuObject.tag == "Ship" &&
                         hitGO.GetComponent<Tile>().occuObject.GetComponent<Ship>().currentPlayerTurn == true && hitGO.GetComponent<Tile>().occuObject.GetComponent<Ship>().state != 3 &&
                         hitGO.GetComponent<Tile>().occuObject != currentShip)
                {
                    ClearTiles();
                    currentShip = hitGO.GetComponent<Tile>().occuObject; //switch operating ship to new
                    currentTile = currentShip.GetComponent<Ship>().tile; //update current tile
                    UpdateTiles(currentShip.GetComponent<Ship>().curActionPoints, 1); //update tiles for new ship
                    return;
                }

                if (hit.collider.tag == "Tile") //if ray hits and tag is a "tile"
                {
                    //hit.collider.gameObject now refers to the tile under the mouse cursor
                    if (hitGO.GetComponent<Tile>().state == 1 || hitGO.GetComponent<Tile>().state == 4)
                    {
                        MoveShip(hitGO); //move the ship if tile state is 1
                        state = 999;
                    }
                    else if (hitGO.GetComponent<Tile>().state == 3 && hitGO.GetComponent<Tile>().IsAdjacent(currentTile))
                    {
                        currentShip.GetComponent<Ship>().SetState(3);
                        ClearTiles();
                        currentShip = null;
                        return;
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
                //Debug.Log("damage has happened");

                if (hit.collider.tag == "Button")
                {
                    return;
                }

                if (hitGO.tag == "Ship" && hitGO.GetComponent<Ship>().currentPlayerTurn == false && hitGO.GetComponent<Ship>().tile.GetComponent<Tile>().state == 2)
                {
                    //Debug.Log("11dmg " + currentShip.GetComponent<Ship>().damage);
                    hitGO.GetComponent<Ship>().GetDamaged(currentShip.GetComponent<Ship>().damage);
                    //Debug.Log("dmg " + currentShip.GetComponent<Ship>().damage);
                    state = 1;
                    currentShip.GetComponent<Ship>().hasAttacked = true;
                    currentShip.GetComponent<Ship>().curActionPoints--;
                    return;
                }
                if (hitGO.tag == "Tile" && hitGO.GetComponent<Tile>().isOccupied == true && hitGO.GetComponent<Tile>().state == 2)
                {
                    //Debug.Log("11dmg " + currentShip.GetComponent<Ship>().damage);
                    hitGO.GetComponent<Tile>().occuObject.GetComponent<Ship>().GetDamaged(currentShip.GetComponent<Ship>().damage);
                    //Debug.Log("dmg " + currentShip.GetComponent<Ship>().damage);
                    state = 1;
                    currentShip.GetComponent<Ship>().hasAttacked = true;
                    currentShip.GetComponent<Ship>().curActionPoints--;
                    return;
                }
            }
        }

        /*
         * Update during movement
         */
        if (state == 998)
        {
            return;
        }
        if (state == 999)
        {
            Vector3 newPosition = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + 0.1f, currentTile.transform.position.z);
            if (currentShip.transform.position != newPosition)
                currentShip.transform.position = Vector3.MoveTowards(currentShip.transform.position, newPosition, 5f * Time.deltaTime);
            else
            {

                UpdateFOW();
                List<int> targets = FindTargets(currentShip.GetComponent<Ship>().atkRange, currentTile);
                SetTargets(targets);

                state = 1;
            }
            return;
        }
        UpdateText();
    }
}
