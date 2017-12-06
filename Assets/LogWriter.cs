using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LogWriter : NetworkBehaviour {
	
	public List<string> messages = new List<string>();
	public string lastMessage;

	[SerializeField]
	private string inputMsg;
	[SerializeField]
	private string tmpName;

	private PlayerToLog player;

	// Use this for initialization
	void Start () {
		//To handle out of range index error in init
		for (int i = 0; i < 10 ; i++) {
			messages.Add (" ");
		}
	}

	public void SetChatPlayer(PlayerToLog p, string name)
	{
		Debug.Log ("here");
		player = p;
		tmpName = name;
	}

	public void AddNameAndMsg(string name, string msg)
	{
		string str = name + ": "+msg;

		if(isServer){
			RpcAddMsg(str);
		}
	}

	// server -> client
	[ClientRpc]
	void RpcAddMsg(string str)
	{
		//Adds message to list
		messages.Add (str);
		string newMessageString = "";
		lastMessage = str;

		//Writes the list to string
		/*for (int i = 9; i > 0 ; i--) {
			newMessageString += "\n " + messages [messages.Count - i];
		}

		//Adds text to gameObject
		gameObject.GetComponentInChildren<Text>().text = newMessageString;*/
	}

	/*void OnGUI()
	{

		inputMsg = GUILayout.TextField(inputMsg);
		if( GUILayout.Button("SEND") ){
			player.SendMsg(inputMsg);
			inputMsg = "";
		}

	}*/

	public bool CheckSameName(string name)
	{
		PlayerToLog[] players = GameObject.FindObjectsOfType<PlayerToLog>();
		for(int i = 0; i < players.Length; i++){
			if(players[i].userName == name){
				return true;
			}
		}
		return false;
	}

	public override void OnNetworkDestroy()
	{
		// disconnect
		Debug.Log("MainUI OnNetworkDestroy");
		NetworkServer.DisconnectAll();
	}

   /* //[SyncVar(hook = "whoAreYou")]
    public string syncedString = "A monkey walked into a park";

	public string blabla = "hej";


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
        {
			whoAreYou ();
        }
            

        //writeToLog(syncedString);
    }

    void whoAreYou()
    {
        if (isServer)
            syncedString = "The server made this A DONKEKY";
        else
            CmdChangeSyncedString();
    }

    [Command]
    void CmdChangeSyncedString()
    {
        syncedString = "The client made this a grizzly moonkey";
    }


    [Command]
    void CmdwriteToLogDefault()
    {
        if (isServer)
        {

        }


        string newMessageString = "Default Message";

        //Adds text to gameObject
        gameObject.GetComponent<Text>().text = newMessageString;
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
	} */
}
