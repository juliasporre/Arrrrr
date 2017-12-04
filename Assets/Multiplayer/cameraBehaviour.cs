using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0)
        {
            GameObject.Find("MainCamera").transform.Rotate(new Vector3(0,10,0));
        }
	}
}
