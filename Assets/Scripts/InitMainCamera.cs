using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class InitMainCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponentInChildren<BackgroundPlaneBehaviour>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
