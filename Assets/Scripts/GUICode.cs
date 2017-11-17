using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUICode : MonoBehaviour {

	public GameObject cube;
	public GameObject mrRoboto;
	public GameObject UserDisplay;
	public GameObject explosion;
	public Font font; 
	public GameObject Cannon;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI () {
		//GUI.Box (new Rect (10, 10, 100, 90), "GUI.Box");
		if (GUI.Button (new Rect (20, 60, 80, 100), "Get distance")) {
			Debug.Log ("Button1 is pressed");
			//cube = GameObject.Find ("TestCube");
			//mrRoboto = GameObject.Find ("mrRoboto");
			//UserDisplay = GameObject.Find ("UserDisplay");

			// Gets a vector that points from the player's position to the target's.
			var heading = mrRoboto.transform.position - cube.transform.position;

			var distance = heading.magnitude;
			var direction = heading / distance; // This is now the normalized direction.

			Text myText = UserDisplay.AddComponent<Text>();
			myText.text = "cubes position is: " + cube.transform.position + "Mr Robotos position is: " + mrRoboto.transform.position + "\n Heading: " + heading + "Distance: " + distance + "Direction: " + direction;
			myText.font = font;
			myText.color = Color.red;
			myText.fontSize = 20;
			myText.transform.position = new Vector3(-600,-220,0);
			Debug.Log ("cubes position is: " + cube.transform.position);
			Debug.Log ("Mr Robotos position is: " + mrRoboto.transform.position);
			Debug.Log ("Heading: " + heading);
			Debug.Log ("Distance: " + distance);
			Debug.Log ("Direction: " + direction);

		}

		if (GUI.Button (new Rect (20, 200, 80, 100), "Attack")) {
			Debug.Log ("ATTACK");
			explosion.SetActive (true);


		}

		if (GUI.Button (new Rect (20, 340, 80, 100), "Place")) {
			Debug.Log ("Placing");
			Cannon.transform.parent = null;
			Cannon.isStatic = true;


		}

	}
}
