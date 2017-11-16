using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameLogic : MonoBehaviour {

	string player; //This player
	public GameObject UserDisplay;
	string url = "http://localhost:9001/test";




	void Start()
	{
		StartCoroutine(SendTest(2f)); //when starting the game, wait 20 sec to position the boats on the board
	}

	IEnumerator SendTest(float time)
	{
		yield return new WaitForSeconds(time);
		string message = "test";
		string toSend = url + "?init=" + message;
		Debug.Log("Sending" + toSend);
		WWW www = new WWW(toSend);
		yield return www;

		player = www.text;
		Debug.Log("got back" + player);
	}
}

