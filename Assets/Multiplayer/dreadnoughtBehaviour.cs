
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dreadnoughtBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawRay(gameObject.transform.position, -gameObject.transform.up, Color.red);

	}

    public void snapShip ()
    {
        RaycastHit hit;

        if (Physics.Raycast(gameObject.transform.position, -gameObject.transform.up, out hit, 100f))
        {
            gameObject.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            gameObject.transform.parent = GameObject.Find("Plane").transform;
            
        }
    }
}
