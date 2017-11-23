using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Select : MonoBehaviour {
	public bool selected;

	private Vector3 currentPosition;
	private Vector3 newPosition;

	private int actionPoints;
	private int speed;
	private int hp;
	private int turnRate;

	// Use this for initialization
	void Start () {
		selected = false;
		currentPosition = this.transform.position;
		newPosition = currentPosition;
		Debug.Log(currentPosition);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		// code to use on mobile phones
		/*if (Input.touchCount > 0) {
			//newCoords = Input.mousePosition.normalized;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
			if(Physics.Raycast(ray, out hit, 100.0f)) {
				selected = true;
				Debug.Log(selected);			
			}
		}*/
		// testing on pc
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 100.0f) && hit.transform.tag == "Player") {
				// deselect every ship first
				foreach (GameObject ship in GameObject.FindGameObjectsWithTag("Player")) {
					ship.GetComponent<Select>().selected = false;
				}
				hit.transform.gameObject.GetComponent<Select>().selected = true;
				//selected = true;
				Debug.Log(selected);
				// create buttons upon selecting a ship
				/*CreateButton(GetComponentInParent<Canvas>().transform, new Vector3(currentPosition.x+5, currentPosition.y,currentPosition.z), new Vector2(5f,5f), turnPort());			
				CreateButton(GetComponentInParent<Canvas>().transform, new Vector3(currentPosition.x-5, currentPosition.y,currentPosition.z), new Vector2(5f,5f), turnStarboard());*/
			}
			// move to target if ship is selected
			else if (selected == true && hit.transform.tag == "Map") {
				Debug.Log("move to " + hit.point.ToString());
				newPosition = hit.point;
				newPosition.y = 1;
			}
			// deselect ship
			// not working atm, maybe set selected to false on all ships and then true onclicked ship
			else {
				selected = false;
				Debug.Log(selected);	
			}
		}
		// move ship to new location
		if (currentPosition.x != newPosition.x && currentPosition.z != newPosition.z) {
			transform.position = Vector3.MoveTowards(transform.position, newPosition, 10f * Time.deltaTime);
			currentPosition = transform.position;
		}
		//Debug.Log(selected);
	}

	// fix the buttons so that they don't issue any movement orders
	// buttons aren't working atm, the if statements aren't resolved as true even when selected == true for some reason
	public void turnPort () {
		Debug.Log("turn port");
		if (selected) {
			transform.Rotate(0, -45f, 0, Space.World);
		}
	}

	public void turnStarboard () {
		Debug.Log("turn starboard");
		if (selected) {
			Debug.Log("wat");
			transform.Rotate(0, 45f, 0, Space.World);
		}
	}
	// not yet working
	public void CreateButton (Transform panel, Vector3 position, Vector2 size, UnityEngine.Events.UnityAction method) {
	    GameObject button = new GameObject();
	    button.transform.parent = panel;
	    button.AddComponent<RectTransform>();
	    button.AddComponent<Button>();
	    button.transform.position = position;
	    button.GetComponent<RectTransform>().sizeDelta = size;
	    button.GetComponent<Button>().onClick.AddListener(method);
	}
}
