using UnityEngine;
using UnityEngine.Networking;

public class playercontroller : NetworkBehaviour
{
	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        if (Input.touchCount > 0)
            x = 10;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Fire();
		}
	}


	void Fire()
	{
		// Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate(
			bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);        
	}

	public override void OnStartLocalPlayer ()
	{
		GetComponent<MeshRenderer>().material.color = Color.blue;
	}
}