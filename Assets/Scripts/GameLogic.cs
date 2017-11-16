using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/* TODO
 * baka ihop markmiss och markhit
 * 
 * */

public class GameLogic : MonoBehaviour {

	Dictionary<string, List<string>> boatsPos = new Dictionary<string, List<string>>();
	Dictionary<string, List<string>> OpponentsBoatsPos = new Dictionary<string, List<string>>();
	string player; //This player

	public GameObject userDisplay;
	public GameObject fireworks;
	public Rigidbody rocket;

	public GameObject kryss;
	public GameObject ring;

	public GameObject birdaboat;
	public GameObject blackperl;
	public GameObject speedyboat;
	public GameObject supersail;
	public GameObject tinyboat;
	public GameObject whitefang;

	string url = "http://192.168.1.7:8000/game"; //use this if other computer is server 
	//string url = "http://localhost:8000/game";
	private IEnumerator printMessage;
	int t = 0; //Begin the game, waiting for command 0

	public AudioSource countDown;
	public AudioSource roger;
	public AudioSource halleluja;
	public AudioSource alarm;
	public AudioSource cow;

	public AudioSource hitSound;
	public AudioSource missSound;



	void Start()
	{
		StartCoroutine(SendPosBoat(30f)); //when starting the game, wait 20 sec to position the boats on the board
		InvokeRepeating("callHelper",0.5f,0.5f); //many calls
	}

	void callHelper()
	{ //only surves to starts getMessage
		if (player != null) //Dont ask for moves until player has been assigned in SendPosBoat
		{
			printMessage = getMessage();
			StartCoroutine(printMessage);
		}
	}

	IEnumerator getMessage()
	{
		WWW www = new WWW(url + "?t=" + t);
		yield return www;

		if (www.text.Length > 0)
		{
			t++;
			string recivedCommands = www.text;
			Debug.Log(recivedCommands);
			string[] commands = recivedCommands.Split(' '); //recieved commands are seperated by " "

			if(commands.Length > 1)
			{
				makeMove(commands);
			}
		}
	}

	void makeMove(string[] commands)
	{
		if (commands[1] == player && commands[2] == "WIN")
		{
			win();
		}
		else if (commands[1] != player && commands[2] == "WIN")
		{
			loss();
		}
		//If the game has not yet reached end state: commands[4] should be name of opponents boat
		else if (commands.Length > 3 && commands[1] == player && commands[3] == "x") //x = hit
		{
			bombOpponentHit("o" + commands[2]);
			if (OpponentsBoatsPos.ContainsKey(commands[4]))
			{
				List<string> list = OpponentsBoatsPos[commands[4]];
				list.Add("o" + commands[2]);
			}
			else if (OpponentsBoatsPos.ContainsKey(commands[4]) == false)
			{
				List<string> list = new List<string>();
				list.Add("o" + commands[2]);
				OpponentsBoatsPos.Add(commands[4], list);
			}
		}
		else if (commands.Length > 3 && commands[1] == player && commands[3] == "o") //o = miss
		{
			bombOpponentMiss("o" + commands[2]);
		}
		else if (commands.Length > 3 && commands[1] == player && commands[3] == "X") //X = sunk
		{
			if (OpponentsBoatsPos.ContainsKey(commands[4]))
			{
				List<string> list = OpponentsBoatsPos[commands[4]];
				list.Add("o" + commands[2]);
			}
			else if (OpponentsBoatsPos.ContainsKey(commands[4]) == false)
			{
				List<string> list = new List<string>();
				list.Add("o" + commands[2]);
				OpponentsBoatsPos.Add(commands[4], list);
			}
			StartCoroutine(bombOpponentSunk("o" + commands[2], commands[4]));
		}
		else if (commands.Length > 3 && commands[3] == ".")
		{
			Debug.Log("Already attacked");
		}

		//if turn is opponent
		else if (commands.Length > 3 && commands[1] != player && commands[3] == "x") //x = hit
		{
			bombMeHit(commands[2]);
		}
		else if (commands.Length > 3 && commands[1] != player && commands[3] == "o") //o = miss
		{
			bombMeMiss(commands[2]);
		}
		else if (commands.Length > 3 && commands[1] != player && commands[3] == "X") //X = sunk
		{
			bombMeSunk(commands[2]);
		}
	}

	void bombOpponentHit(string bombPos)
	{
		roger.Play();
		Debug.Log("Your attack hit!");
		fireMissle(bombPos);
		StartCoroutine(markHit(bombPos));
	}

	void bombOpponentMiss(string bombPos)
	{
		roger.Play();
		Debug.Log("Your attack missed.");
		fireMissle(bombPos);
		StartCoroutine(markMiss(bombPos));
	}


	void bombMeHit(string bombPos)
	{
		Debug.Log("Your boat was hit!");
		alarm.Play();
		fireMissle(bombPos);
	}

	void bombMeMiss(string bombPos)
	{
		Debug.Log("Your boats was not hit.");
		fireMissle(bombPos);
	}

	IEnumerator bombOpponentSunk(string bombPos, string boat)
	{
		roger.Play();
		StartCoroutine(markHit(bombPos));
		fireMissle(bombPos);
		yield return new WaitForSeconds(6f);
		Debug.Log("You sank the opponents boat!");

		//Calculating placement of the ship
		List<string> posList = OpponentsBoatsPos[boat];
		Vector3 middlepoint = new Vector3(0, 0, 0);
		foreach (string position in posList)
		{
			middlepoint += GameObject.Find(position).transform.position;
		}
		middlepoint = middlepoint / posList.Count;

		GameObject sunkBoat = birdaboat;

		if (boat == "blackperl")
		{
			sunkBoat = blackperl;
		}
		else if (boat == "supersail")
		{
			sunkBoat = supersail;
		}
		else if (boat == "speedyboat")
		{
			sunkBoat = speedyboat;
		}
		else if (boat == "tinyboat")
		{
			sunkBoat = tinyboat;
		}
		else if (boat == "whitefang")
		{
			sunkBoat = whitefang;
		}

		GameObject opponentsSinkingBoat = Instantiate(sunkBoat, middlepoint, transform.rotation);
		opponentsSinkingBoat.transform.SetParent(GameObject.Find("BoardBoxcolliders (Opponent)").transform, false);
		opponentsSinkingBoat.transform.position = middlepoint;
		opponentsSinkingBoat.transform.rotation = Quaternion.Euler(-0.855f, 87.152f, -3.852f);
		opponentsSinkingBoat.GetComponent<Animator>().enabled = true;
	}

	void bombMeSunk(string bombPos)
	{
		Debug.Log("Your boat was sunk.");
		alarm.Play();
		fireMissle(bombPos);
		foreach (string boat in boatsPos.Keys)
		{
			List<string> posList = boatsPos[boat];
			foreach (string position in posList)
			{
				if (position == bombPos)
				{
					GameObject.Find(boat).GetComponent<Animator>().enabled = true;
				}
			}
		}
	}

	IEnumerator markHit (string bombPos)
	{ //Places an O where boat was hit on opponents side
		yield return new WaitForSeconds(3.7f);
		GameObject success = Instantiate(ring, new Vector3(0, 0.4f, 0), new Quaternion(0, 0, 0, 0));
		success.transform.SetParent(GameObject.Find(bombPos).transform, false);
		hitSound.Play();
	}

	IEnumerator markMiss(string bombPos)
	{//Places an X where boat was hit on opponents side
		yield return new WaitForSeconds(3.7f);
		GameObject success = Instantiate(kryss, new Vector3(0, 0.4f, 0), new Quaternion(0, 0, 0, 0));
		success.transform.SetParent(GameObject.Find(bombPos).transform, false);
		missSound.Play();
	}

	void win()
	{
		userDisplay.GetComponent<Text>().text = "CONGRATULATIONS, YOU WON THE GAME!";
		fireworks.SetActive(true);
		halleluja.Play();
	}

	void loss()
	{
		userDisplay.GetComponent<Text>().text = "YOU LOST THE GAME";
		cow.Play();
	}

	void fireMissle(string boxName)
	//creates a missle above the box "A1,A2..." that was sent by command
	{
		Vector3 place = GameObject.Find(boxName).transform.position;
		Rigidbody rocketClone = (Rigidbody)Instantiate(rocket, place, transform.rotation * Quaternion.Euler(90, 0, 0));
		rocketClone.transform.parent = transform; //fäster raketen på board
		rocketClone.transform.position += transform.up * 40f; //vrider raketen med huvet ner
		rocketClone.velocity = -transform.up * 10f; //hastighet nedåt
	}

	void Update()
	{

		if (Input.GetKeyDown("space"))
		{
			countDown.Play();
			GameObject.Find("blackperl").GetComponent<Animator>().enabled = true;
		}

		if (Input.GetKeyDown("up"))
		{
			roger.Play();
			List<string> list = new List<string>();
			list.Add("oA1");
			list.Add("oA2");
			StartCoroutine(markHit("oA1"));
			StartCoroutine(markHit("oA2"));

			OpponentsBoatsPos.Add("speedyboat", list);
		}
		if (Input.GetKeyDown("down"))
		{
			win();
			StartCoroutine(bombOpponentSunk("oA3", "speedyboat"));
		}
	}

	public void OnChildsTriggerEnter(string name, Collider other) // Other = Gameobject boat
	{
		Debug.Log("Placed a boat named: " + other.name);
		if (boatsPos.ContainsKey(other.name))
		{
			List<string> list = boatsPos[other.name];
			if (list.Contains(name) == false)
			{
				list.Add(name);
			}
		}
		else
		{
			List<string> list = new List<string>();
			list.Add(name);
			boatsPos.Add(other.name, list);
		}
	}


	public void OnChildsTriggerExit(string name, Collider other)
	{
		Debug.Log("Removed " + other.name + " from " + name);
		if (boatsPos.ContainsKey(other.name))
		{
			List<string> list = boatsPos[other.name];
			if (list.Contains(name) && list.Count > 1)
			{
				list.Remove(name);
			}
			else if (list.Contains(name) && list.Count == 1)
			{
				list.Remove(other.name);
			}
		}
	}

	IEnumerator SendPosBoat(float time)
	{
		StartCoroutine(StartCountDown());
		//Gets the position of each boat
		yield return new WaitForSeconds(time);
		//GetComponent<AudioSource>().Play();


		string listToSend = "";

		foreach (string boat in boatsPos.Keys)
		{
			GameObject fixedboat = GameObject.Find(boat);
			listToSend += boat + "_";
			List<string> posList = boatsPos[boat];
			string positions = "";

			foreach (string position in posList)
			{
				listToSend +=  position;
				positions += position;
			}

			posList.Sort();
			Debug.Log(positions);
			bool horizontal = System.Text.RegularExpressions.Regex.IsMatch(positions, "^(((A\\d)+)|((B\\d)+)|((C\\d)+)|((D\\d)+)|((E\\d)+))$");
			Debug.Log("horizontal:" + horizontal);

			if (posList.Count % 2 == 1)
			{
				fixedboat.transform.SetParent(GameObject.Find(posList[posList.Count / 2]).transform, false);
				fixedboat.transform.position = GameObject.Find(posList[posList.Count / 2]).transform.position;
				fixedboat.transform.rotation = Quaternion.Euler(0, 0, 0);
			}
			else if (posList.Count % 2 == 0)
			{
				fixedboat.transform.SetParent(GameObject.Find(posList[posList.Count / 2]).transform, false);
				fixedboat.transform.position = GameObject.Find(posList[posList.Count / 2]).transform.position;
				fixedboat.transform.rotation = Quaternion.Euler(0, 0, 0);
			}
			if (horizontal)
			{
				fixedboat.transform.rotation = Quaternion.Euler(0, 90, 0);
				if(posList.Count % 2 == 0)
				{
					fixedboat.transform.position += new Vector3(0,0,1); 
				}
			}
			else
			{
				if (posList.Count % 2 == 0)
				{
					fixedboat.transform.position += new Vector3(1, 0, 0);
				}
			}
			fixedboat.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
			fixedboat.transform.position += new Vector3(0, 1f, 0);



			listToSend += "_";
		}
		listToSend = listToSend.Remove(listToSend.Length - 1);
		//listToSend = "blackperl_A1B1xtitanic_D2D3D4"; //position of ship for testing

		string urlBoats = url + "?init=" + listToSend;
		Debug.Log("Sending " + urlBoats);
		WWW www = new WWW(urlBoats);
		yield return www;

		player = www.text;
		Debug.Log("You are now " + player);

		var allColliders = GetComponentsInChildren<Collider>();
		foreach (var childCollider in allColliders)
		{
			childCollider.isTrigger = true;
		}
	}

	IEnumerator StartCountDown()
	{
		yield return new WaitForSeconds(18);
		countDown.Play();

	}
}

