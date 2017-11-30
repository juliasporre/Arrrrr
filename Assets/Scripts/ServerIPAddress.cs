using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerIPAddress : MonoBehaviour {


	static string serverAddress = "";

	public void saveServerIP (){
		serverAddress = GameObject.Find("IPText").GetComponent<Text>().text;
		GameObject.Find ("IPField").GetComponent<Text>().text += "\n Server IP: " + serverAddress;

	}
}
