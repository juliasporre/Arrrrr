using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {
    public GameObject tile;

    public int indexNumber;
    public int actionPoints;

    //private float yOffset = 0.35f;

    // Use this for initialization
    void Start ()
    {
        gameObject.name = "Ship " + indexNumber.ToString();
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}
