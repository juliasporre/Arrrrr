using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour {
    public GameObject tile;
    public GameObject statsMesh;

    public int indexNumber;

    public int iniActionPoints;
    public int curActionPoints;

    public int atkRange = 3;
    public int health = 15;
    public int damage = 3;

    private TextMesh stats;

    //private float yOffset = 0.35f;

    // Use this for initialization
    void Start ()
    {
        gameObject.name = "Ship " + indexNumber.ToString();
        curActionPoints = iniActionPoints;
        var statsClone = Instantiate(statsMesh, transform);
        stats = statsClone.GetComponent<TextMesh>();
        stats.text = "HP: " + health.ToString() + "\nAP: " + curActionPoints.ToString();
        stats.GetComponent<Renderer>().enabled = true;
    }
	
    public void GetDamaged(int damage)
    {
        health = health - damage;
        stats.text = "HP: " + health.ToString() + "\nAP: " + curActionPoints.ToString();
    }

    private void UpdateText()
    {
        stats.text = "HP: " + health.ToString() + "\nAP: " + curActionPoints.ToString();
    }

	// Update is called once per frame
	void Update ()
    {
        UpdateText();
    }
}
