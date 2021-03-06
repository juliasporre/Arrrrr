﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System;

public class GridGenerator : NetworkBehaviour
{
    
    public PlayerToLog shortcut;
	public LogWriter lw;
	int messageCounter = -1;

    public GameObject tilePrefab;
    public GameObject playerPrefab;
    public GameObject islandPrefab;
    public Text uiText;
    //public Text buttonText;
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
    public int myPlayerNumber;
    
    // Use this for initialization
    void Start()
    {
        startGame();
        uiText = GameObject.Find("uiText").GetComponent<Text>();
    }

    void startGame()
    {
        this.name = "Grid";
		lw = GameObject.Find("LogCanvas").GetComponent<LogWriter>();

		if (shortcut.nameStr == "user_Server")
			myPlayerNumber = 0;
		else {
			myPlayerNumber = 1;
			messageCounter = 0;
		}
        Debug.Log("Server says I'm: " + shortcut.nameStr);
        Debug.Log("My player number is: " + myPlayerNumber.ToString());
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
                newIsland.transform.position = new Vector3(newTile.transform.position.x + .5f*transform.localScale.x, newTile.transform.position.y + 0.2f, newTile.transform.position.z + 1f*transform.localScale.z);
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
        playerArray[1].transform.rotation = new Quaternion(0, -90, 0, 1);
            //currentShip = playerArray[currentPlayer+1].GetComponent<Players>().firstShip;
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
    * A recursive function that returns an array of indices of tiles were isOccupied is true within the specified range
    * will however mark every occupied square in range as a possible target
    */
    private List<int> FindTargets(int range, GameObject tile)
    {
        List<int> foundObjects = new List<int>();

        if (currentTile != null)
        {
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
            currentTile = playerArray[myPlayerNumber].GetComponent<Players>().shipArray[0].GetComponent<Ship>().tile;
        List<int> cTL = new List<int>();
        foreach (GameObject ship in playerArray[myPlayerNumber].GetComponent<Players>().shipArray)
        {
            var shipT = ship.GetComponent<Ship>().tile;
            cTL.AddRange(FindTargets(fogOfWarRange, shipT));
        }

        foreach (GameObject player in playerArray)
        {
            var players = player.GetComponent<Players>();

            if (players.playerNumber != myPlayerNumber)
            {
                players.FogOfWar(cTL); //Call FogOfWar method if player is not CurrentPlayer.
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
            }
            else
            {
                state = 1;
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
		//buttonText.text = "Pass Turn";
		currentPlayer++;

		if (CheckVictory()) //if victory conditions are met
			return; //finish game immediately
		if (currentPlayer+1 > numberOfPlayers)
		{
			currentPlayer = 0;
		}

		//Debug.Log ("In pass turn, i'm player " + myPlayerNumber + " And shortcut.currentplayer is " + currentPlayer);

        
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
        state = 1; //movement phase
    }

    void OnGUI() {
        var sW = Screen.width;
        var sH = Screen.height;
        GUIStyle gS = GUI.skin.GetStyle("Button");
        gS.fontSize = 34;
        if (currentPlayer == -1)
        {
            if (GUI.Button(new Rect(sW - sW/6, sH - sH / 6, sW/6, sH / 6), "Start Game"))
            {
                SendEndTurnMessage();
            }
        }
        if (currentPlayer == myPlayerNumber)
        { 
            if (GUI.Button(new Rect(sW - sW / 6, sH - sH / 6, sW / 6, sH / 6), "Pass")) {
			    SendEndTurnMessage();
            }
            if (GUI.Button(new Rect(sW - sW / 6, sH - 2*sH / 6, sW / 6, sH / 6), "Attack"))
            {
			    AttackButton();
            }
            if (state == 2)
            {
                if (GUI.Button(new Rect(sW - sW / 6, sH - 3 * sH / 6, sW / 6, sH / 6), "Close Range"))
                {
                    currentShip.GetComponent<Ship>().ChangeWeapons(0);
                    ClearTiles();
                }
                if (GUI.Button(new Rect(sW - sW / 6, sH - 4 * sH / 6, sW / 6, sH / 6), "Long Range"))
                {
                    currentShip.GetComponent<Ship>().ChangeWeapons(1);
                    ClearTiles();
                }
            }
        }
    }

	public void SendEndTurnMessage() {
		Debug.Log ("I'm sending a end trun message");
		int messageNumber = messageCounter + 1;
		shortcut.SendMsg (messageNumber + " nextturn");
	}

	void readLastMessage(){
		string newMessage = lw.lastMessage;
		string[] array = newMessage.Split(' ');

		//if (messageCounter < array 
		try{ 

			if (array.Length > 1 && array[0] != shortcut.userName && Int32.Parse(array[1]) > messageCounter) {
				Debug.Log (newMessage);
				messageCounter ++;
				decryptMessage(array);
			}
		} catch (FormatException e){
			Debug.Log("Something wrong");
		}

	}

	void decryptMessage(string[] message){
		if (message [2] == "nextturn") {
			Debug.Log ("decrypt success next turn");
			EndTurn (); //true is set to tell it is only an update from server
		} else if (message [2] == "move") { // username, messageNumber, "moved SHIPNAME TILENAME"
			Debug.Log ("decrypt success move");
			MoveShip (message [3],message [4]); // Pass variables
		} else if (message [2] == "attack") { // username, messageNumber, "attack SHIPNAME attacking SHIPNAME2 withDamage DAMAGE"
			Debug.Log ("decrypt success attack");
			AttackShip (message [3],message [5],message[7]); // Pass variables
		}
	}

	void AttackShip(string attackingShip, string attackedShip, string damage){
		
		AttackShip (GameObject.Find (attackingShip), GameObject.Find (attackedShip), damage);

	}

	void MoveShip(string movedShip, string tile){

		MoveShip (GameObject.Find (movedShip), GameObject.Find (tile));

	}


	void AttackShip(GameObject attackingShip, GameObject attackedShip, string damage)
	{
		//Run code from update
		Debug.Log("Now " + attackedShip.name + "is being attacked");

		attackedShip.GetComponent<Ship>().GetDamaged(Int32.Parse(damage));
	}


	/*
     * Function for moving ship to selected newTile.
     */
	void MoveShip(GameObject cShip, GameObject newTile)
	{
		if (cShip == currentShip) {
			cShip.GetComponent<Ship> ().tile.GetComponent<Tile> ().isOccupied = false;
			cShip.GetComponent<Ship> ().tile.GetComponent<Tile> ().occuObject = null;

            Vector3 originPos = cShip.GetComponent<Ship>().tile.transform.position;


            cShip.GetComponent<Ship> ().tile = newTile; //updates currentShip tile to newTile
			cShip.GetComponent<Ship> ().curActionPoints = newTile.GetComponent<Tile> ().remAP;
			currentTile = newTile; //updates currentTile to newTile
			newTile.GetComponent<Tile> ().isOccupied = true;
			newTile.GetComponent<Tile> ().occuObject = cShip; //indicate new tile is occupied

            Vector3 newPos = newTile.transform.position; 
            Vector3 relativePos = newPos - originPos;

            Vector3 newPosition = new Vector3 (currentTile.transform.position.x, currentTile.transform.position.y + 0.1f, currentTile.transform.position.z);
			while (currentShip.transform.position != newPosition)
				currentShip.transform.position = Vector3.MoveTowards (currentShip.transform.position, newPosition, 5f * Time.deltaTime);

            Debug.Log("Moving in direction: " + relativePos);
            currentShip.transform.rotation.SetLookRotation(Vector3.Normalize(relativePos));
            //currentShip.transform.rotation.SetLookRotation(relativePos, Vector3.up);
            UpdateFOW ();
			List<int> targets = FindTargets (currentShip.GetComponent<Ship> ().atkRange, currentTile);
			SetTargets (targets);

			state = 1;
			ClearTiles ();
			return;
		} else {
			cShip.GetComponent<Ship> ().tile.GetComponent<Tile> ().isOccupied = false;
			cShip.GetComponent<Ship> ().tile.GetComponent<Tile> ().occuObject = null;

			cShip.GetComponent<Ship> ().tile = newTile; //updates currentShip tile to newTile
			cShip.GetComponent<Ship> ().curActionPoints = newTile.GetComponent<Tile> ().remAP;
			currentTile = newTile; //updates currentTile to newTile
			newTile.GetComponent<Tile> ().isOccupied = true;
			newTile.GetComponent<Tile> ().occuObject = cShip; //indicate new tile is occupied

			Debug.Log("MoveClient");
			Vector3 newPosition = new Vector3 (currentTile.transform.position.x, currentTile.transform.position.y + 0.1f, currentTile.transform.position.z);
			currentShip = cShip;
			while (currentShip.transform.position != newPosition)
				currentShip.transform.position = Vector3.MoveTowards (currentShip.transform.position, newPosition, 5f * Time.deltaTime);

			UpdateFOW ();
			List<int> targets = FindTargets (currentShip.GetComponent<Ship> ().atkRange, currentTile);
			SetTargets (targets);

			state = 1;
		}
		ClearTiles(); //set state=1 tiles to 0.


	}

    // Update is called once per frame
    void Update()
    {

        readLastMessage ();

        if (currentPlayer != myPlayerNumber)
        {
            Debug.Log("Not my turn" + shortcut.currentPlayer);
            return;
        }
        //Ray shoots from camera POV
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction);
        RaycastHit hit;
        /*
        if (currentShip != null && currentShip.GetComponent<Ship>().state != 3)
        {
            currentTile = currentShip.GetComponent<Ship>().tile;
            iniActionPoints = currentShip.GetComponent<Ship>().iniActionPoints;
            List<int> targets = FindTargets(currentShip.GetComponent<Ship>().atkRange, currentTile);
            SetTargets(targets);
            currentShip.GetComponent<Ship>().stats.GetComponent<Renderer>().enabled = true;
        }*/
        if (state == 1) //movement state
        {
            if (currentShip != null)
            {
                //currentShip.GetComponent<Ship>().curActionPoints = iniActionPoints;
                UpdateTiles(currentShip.GetComponent<Ship>().curActionPoints, 1);
                //Debug.Log("NOT EMPTY");
            }
            if (Input.GetButtonDown("Fire1") && Physics.Raycast(ray, out hit)) //On Mouse Click
            {
                Debug.Log("HIT");
                GameObject hitGO = hit.collider.gameObject;
				if (currentShip != null)
					currentTile = currentShip.GetComponent<Ship> ().tile;
				/*
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    Debug.Log("Button has press");
                    //return;
                }*/
                if (hit.collider.tag == "Ship" && hitGO.GetComponent<Ship>().ownerNumber == myPlayerNumber && hitGO != currentShip && hitGO.GetComponent<Ship>().state != 3)
                {
                    ClearTiles();
                    currentShip = hitGO; //switch operating ship to new
                    currentTile = currentShip.GetComponent<Ship>().tile; //update current tile
                    UpdateTiles(currentShip.GetComponent<Ship>().curActionPoints, 1); //update tiles for new ship
                    return;
                }
                else if (hit.collider.tag == "Tile" && hitGO.GetComponent<Tile>().isOccupied && hitGO.GetComponent<Tile>().occuObject.tag == "Ship" &&
                         hitGO.GetComponent<Tile>().occuObject.GetComponent<Ship>().ownerNumber == myPlayerNumber && hitGO.GetComponent<Tile>().occuObject.GetComponent<Ship>().state != 3 &&
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
						int messageNumber = messageCounter + 1;
                        //MoveShip(currentShip, hitGO); //move the ship if tile state is 1
						shortcut.SendMsg (messageNumber + " move " + currentShip.name + " " + hitGO.name);
						//messageCounter++;
						currentTile = hitGO;
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
            UpdateTiles(currentShip.GetComponent<Ship>().atkRange, 2); //shows range of ship
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
					int messageNumber = messageCounter + 1;
					string msg = messageNumber + " attack " + currentShip.name + " attacking " + hitGO.GetComponent<Ship> ().name + " withDamage " + currentShip.GetComponent<Ship>().damage;
					shortcut.SendMsg(msg);
					//Debug.Log("11dmg " + currentShip.GetComponent<Ship>().damage);
                    //hitGO.GetComponent<Ship>().GetDamaged(currentShip.GetComponent<Ship>().damage);
                    //Debug.Log("dmg " + currentShip.GetComponent<Ship>().damage);
                    state = 1;
                    currentShip.GetComponent<Ship>().hasAttacked = true;
                    currentShip.GetComponent<Ship>().curActionPoints--;
                    return;
                }
                if (hitGO.tag == "Tile" && hitGO.GetComponent<Tile>().isOccupied == true && hitGO.GetComponent<Tile>().occuObject.GetComponent<Ship>().ownerNumber != myPlayerNumber && hitGO.GetComponent<Tile>().state == 2)
                    {
                    int messageNumber = messageCounter + 1;
					string msg = messageNumber + " attack " + currentShip.name + " attacking " + hitGO.GetComponent<Tile> ().occuObject.GetComponent<Ship> ().name + " withDamage " + currentShip.GetComponent<Ship>().GetComponent<Ship>().damage;
                    shortcut.SendMsg(msg);
                    //Debug.Log("11dmg " + currentShip.GetComponent<Ship>().damage);
                    //hitGO.GetComponent<Tile>().occuObject.GetComponent<Ship>().GetDamaged(currentShip.GetComponent<Ship>().damage);
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
		if (state == 999) {
			return;
		}
        UpdateText();
        if (currentShip != null && currentShip.GetComponent<Ship>().ownerNumber != myPlayerNumber)
        {
            currentShip = null;
        }
    }
}
