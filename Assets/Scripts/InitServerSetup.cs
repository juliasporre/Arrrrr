using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InitServerSetup : NetworkBehaviour {

	NetworkManager nm;
	ServerIPAddress sIP;
	// Use this for initialization
	void Start () {
		nm = GameObject.Find ("Network manager").GetComponent<NetworkManager>();
		sIP = GameObject.Find ("ServerIPAddressHandeler").GetComponent<ServerIPAddress> ();
		nm.networkAddress = ServerIPAddress.serverAddress;

		if(ServerIPAddress.serverAddress == "localhost")
			GetComponent<NetworkManagerHUD> ().manager.StartHost ();
		else
			Debug.Log("in else");
		GetComponent<NetworkManagerHUD> ().manager.StartClient(nm.matchInfo);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
