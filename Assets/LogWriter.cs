using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LogWriter : NetworkBehaviour {


    [SyncVar]
    public string syncedString = "A monkey walked into a park";

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
        if (Input.GetKey(KeyCode.Space))
            syncedString = "A MONKEY!!!!!";

        writeToLog(syncedString);
    }

	void writeToLog(string message){

		//Adds message to list
		messages.Add (message);
		string newMessageString = "";

		//Writes the list to string
		for (int i = 9; i > 0 ; i--) {
			newMessageString += "\n " + messages [messages.Count - i];
		}

		//Adds text to gameObject
		gameObject.GetComponent<Text>().text = newMessageString;
	}
}
