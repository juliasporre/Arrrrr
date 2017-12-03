﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Players : MonoBehaviour {

    public GameObject shipPrefab;

    //public static GameObject[] shipArray;
    public List<GameObject> shipArray;
    public GameObject[] tileArray;
    public int numberOfShips;
    public int iniActionPoints;

    public GameObject firstShip;
    public GameObject currentTile;

    public int playerNumber;

    // Use this for initialization
    void Start()
    {
        CreateShips();
    }

    void CreateShips()
    {
        int spawnInd = 1;
        if (playerNumber == 1)
        {
            spawnInd = tileArray.Length-3;
        }
        gameObject.name = "Player " + playerNumber.ToString();
        //create ship array for potential multiple ships
        float yOffset = 0.35f;
        for (int shipsCreated = 0; shipsCreated < numberOfShips; shipsCreated++)
        {
            //instantiate new ship
            var newShip = Instantiate(shipPrefab, transform);
            //assign variables to new ship
            currentTile = tileArray[spawnInd++];
            currentTile.GetComponent<Tile>().isOccupied = true;
            currentTile.GetComponent<Tile>().occuObject = newShip;
            newShip.GetComponent<Ship>().tile = currentTile;
            //assign position to ship
            newShip.transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y + yOffset, currentTile.transform.position.z);
            newShip.GetComponent<Ship>().iniActionPoints = iniActionPoints;
            newShip.GetComponent<Ship>().indexNumber = shipsCreated;
            //insert into shipArray
            shipArray.Add(newShip);
        }
        firstShip = shipArray[0];
        currentTile = shipArray[0].GetComponent<Ship>().tile;
    }

    public void SetTurn(int next)
    {
        if (next == playerNumber)
        {
            foreach (GameObject thisShip in shipArray)
            {
                var shipScript = thisShip.GetComponent<Ship>();
                shipScript.UpdateState(true);
                shipScript.curActionPoints = iniActionPoints;
                shipScript.UpdateTimer();
                shipScript.hasAttacked = false;
            }
        }
        else
        {
            foreach (GameObject thisShip in shipArray)
            {
                thisShip.GetComponent<Ship>().UpdateState(false);
            }
        }

    }

    public bool CheckShips()
    {
        if (shipArray.Count > 0)
        {
            return false;
        }
        return true;
    }

    public bool CheckTreasure()
    {
        foreach (GameObject ship in shipArray)
        {
            var ships = ship.GetComponent<Ship>();
            if (ships.tile.GetComponent<Tile>().state == 4)
                return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update () {
        foreach (GameObject thisShip in shipArray)
        {
            var shipscript = thisShip.GetComponent<Ship>();
            if (shipscript.CheckHealth() == false)
            {
                shipArray.Remove(thisShip);
            }
        }
    }
}
