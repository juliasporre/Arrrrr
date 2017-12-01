using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Players : MonoBehaviour {

    public GameObject shipPrefab;

    public static GameObject[] shipArray;
    public GameObject[] tileArray;
    public int numberOfShips;
    public int iniActionPoints;

    public GameObject currentShip;
    public GameObject currentTile;

    public int playerNumber;

    // Use this for initialization
    void Start()
    {
        CreateShips();
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
            //currentShip = newShip;
            newShip.GetComponent<Ship>().iniActionPoints = iniActionPoints;
            newShip.GetComponent<Ship>().indexNumber = shipsCreated;
            //insert into shipArray
            shipArray[shipsCreated] = newShip;
        }
        // set currentPlayerTurn to true for 1 of the ships, otherwise this must be set in the inspector
        shipArray[0].GetComponent<Ship>().currentPlayerTurn = true;
    }

    public void setTurn(int nextPlayer)
    {
        foreach (GameObject ship in shipArray)
        {   
            if (ship.GetComponent<Ship>().currentPlayerTurn && (nextPlayer != playerNumber))
            {
                ship.GetComponent<Ship>().currentPlayerTurn = false;
            }
            else if (nextPlayer == playerNumber)
            {
                ship.GetComponent<Ship>().currentPlayerTurn = true;
            }
}
    }

    // Update is called once per frame
    void Update () {
		
	}
}
