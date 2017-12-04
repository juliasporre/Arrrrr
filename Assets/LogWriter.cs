using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LogWriter : MonoBehaviour {

	List<string> messages = new List<string>();

	void Start () {

		//To handle out of range index error in init
		for (int i = 0; i < 10 ; i++) {
			messages.Add (" ");
		}

		//First message
		writeToLog("Game started");
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
