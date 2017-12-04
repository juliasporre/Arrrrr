using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerIPAddress : MonoBehaviour {


	static string serverAddress = "";
	static bool isServer = false;
	string YourIP;

	void Start () {
		YourIP = Network.player.ipAddress;

	}

	public void saveServerIP (){

		serverAddress = GameObject.Find("IPText").GetComponent<Text>().text;
		isServer = false;
		GameObject.Find ("isClientText").GetComponent<Text>().text = "You are connected to: " + serverAddress;

	}

	public void setHost(){

		if (isServer) {
			isServer = false;
			GameObject.Find ("HostText").GetComponent<Text>().text = "Host game";
			GameObject.Find ("isHostText").GetComponent<Text>().text = "";

		} else {
			isServer = true;
			GameObject.Find ("HostText").GetComponent<Text>().text = "Cancel hosting";
			GameObject.Find ("isHostText").GetComponent<Text>().text = "You are host of the game \n Your IP adress is: " + YourIP;
			serverAddress = "localhost";
		}

	}
}
