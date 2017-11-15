using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour {
	
	//public Transform explosionPrefab;

	void OnTriggerEnter(Collider other)
	{
		//Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
		//Vector3 pos = contact.point;
		//Instantiate(explosionPrefab, pos, rot);
		//Destroy(other);
		Debug.Log("collision with: " + other.name);
	}
}
