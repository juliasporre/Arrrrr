using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUICode : MonoBehaviour {

	public GameObject cube;
	public GameObject mrRoboto;
	public GameObject UserDisplay;
	// Use this for initialization
	void Start () {
		Debug.Log ("hej");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI () {
		//GUI.Box (new Rect (10, 10, 100, 90), "GUI.Box");
		if (GUI.Button (new Rect (20, 40, 80, 100), "Get distance")) {
			Debug.Log ("Button1 is pressed");
			cube = GameObject.Find ("TestCube");
			mrRoboto = GameObject.Find ("mrRoboto");
			UserDisplay = GameObject.Find ("UserDisplay");

			// Gets a vector that points from the player's position to the target's.
			var heading = mrRoboto.transform.position - cube.transform.position;

			var distance = heading.magnitude;
			var direction = heading / distance; // This is now the normalized direction.


			GameObject ngo = new GameObject("myTextGO");
			ngo.transform.SetParent(UserDisplay.transform);

			Text myText = ngo.AddComponent<Text>();
			myText.text = "cubes position is: " + cube.transform.position + "Mr Robotos position is: " + mrRoboto.transform.position + "Heading: " + heading + "Distance: " + distance + "Direction: " + direction;

			Debug.Log ("cubes position is: " + cube.transform.position);
			Debug.Log ("Mr Robotos position is: " + mrRoboto.transform.position);
			Debug.Log ("Heading: " + heading);
			Debug.Log ("Distance: " + distance);
			Debug.Log ("Direction: " + direction);

		}
	}
}
