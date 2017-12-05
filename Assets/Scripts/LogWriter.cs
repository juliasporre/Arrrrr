using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LogWriter : NetworkBehaviour {


    [SyncVar]
    public string syncedString = "A monkey walked into a BARK";

    public bool apa = true;
    public bool sopa = false;

    List<string> messages = new List<string>();

	void Start () {

		//To handle out of range index error in init
		for (int i = 0; i < 10 ; i++) {
			messages.Add (" ");
		}

		//First message
		writeToLog("Game started");
	}

    void Update()
    {
        //Debug.Log("UPDAAAATE");

        if (Input.GetKey(KeyCode.Space))
        {
            whoAreYou();
        }
            

        //writeToLog(syncedString);
    }

    void whoAreYou()
    {
        if (isServer)
        {
            Debug.Log("I am server");
            syncedString = "The server made this A DONKEKY";
        }
        else
        {
            Debug.Log("I am client");
            CmdChangeSyncedString();
        }
    }

    [Command]
    void CmdChangeSyncedString()
    {
        syncedString = "The client made this a grizzly moonkey";
    }


    //[Command]
    /*void CmdwriteToLogDefault()
    {
        if (isServer)
        {

        }


        string newMessageString = "Default Message";

        //Adds text to gameObject
        gameObject.GetComponent<Text>().text = newMessageString;
    }*/

    void writeToLog(string message){

		//Adds message to list
		messages.Add (message);
		string newMessageString = "";

		//Writes the list to string
		for (int i = 9; i > 0 ; i--) {
			newMessageString += "\n " + messages [messages.Count - i];
		}

		//Adds text to gameObject
		//gameObject.GetComponent<Text>().text = newMessageString;
        gameObject.GetComponentInChildren<Text>().text = newMessageString;
	}
}
