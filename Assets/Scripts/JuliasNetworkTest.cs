using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JuliasNetworkTest : NetworkBehaviour {


	[SyncVar]
	public bool testvAR = false;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!isServer)
		{
			return;
		}


		if (Input.GetKey(KeyCode.S))
		if(testvAR)
			testvAR = false;
		else
			testvAR = true;

		if (testvAR)
			Debug.Log("NU SYNKAS VÅR VARIABEL DETTA ÄR JULIA");
		
	}
}
