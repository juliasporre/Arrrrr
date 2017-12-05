using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerToLog : NetworkBehaviour {

	[SyncVar]
	public string userName = "user";

	private LogWriter mainUI;

	// Use this for initialization
	void Awake () {
		Debug.Log("Awake");
	}

	public override void PreStartClient()
	{
		Debug.Log("PreStartClient");
		mainUI = GameObject.FindObjectOfType<LogWriter>();
	}




	public override void OnStartLocalPlayer()
	{
		Debug.Log("OnStartLocalPlayer");
		string nameStr = isServer ? "user_Server" : "user_Client";

		mainUI.SetChatPlayer(this, nameStr);

		SendName(nameStr);
		CmdSendToServer("connected.");
	}

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
		Debug.Log("OnNetworkDestroy "+userName);
		SendMsg("disconnected.");
	}

	public void SendMsg(string str)
	{
		CmdSendToServer(str);
	}

	public void SendName(string str)
	{
		CmdSendName(str);
	}

	//[Client]
	public void ChangeName(string str)
	{
		if( mainUI.CheckSameName(str) ){
			SendMsg("same name.");
			return;
		}
		string currentName = userName;

		SendName(str);
		SendMsg(currentName +" > "+ str + " renamed.");
	}

	// client player -> server
	[Command]
	void CmdSendToServer(string str)
	{
		mainUI.AddNameAndMsg(userName, str);
	}

	[Command]
	void CmdSendName(string name)
	{
		userName = name;
	}
}
