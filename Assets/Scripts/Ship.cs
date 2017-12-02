using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour {
    public GameObject tile;
    public GameObject statsMesh;
    public GameObject popMesh;

    public int indexNumber;

    public int iniActionPoints;
    public int curActionPoints;

    public int atkRange = 3;
    public int health = 15;
    public int damage = 3;

    public bool currentPlayerTurn = false;

    public TextMesh stats;
    public TextMesh popText;

    //private float yOffset = 0.35f;

    // Use this for initialization
    void Start ()
    {
        gameObject.name = "Ship " + indexNumber.ToString();
        curActionPoints = iniActionPoints;

        var statsClone = Instantiate(statsMesh, transform);
        stats = statsClone.GetComponent<TextMesh>();
        stats.text = "HP: " + health.ToString() + "\nAP: " + curActionPoints.ToString();
        stats.GetComponent<Renderer>().enabled = false;

        var popClone = Instantiate(popMesh, transform);
        popText = popClone.GetComponent<TextMesh>();
        popText.GetComponent<Renderer>().enabled = false;
    }

    /*
     * Shows string message with delay in seconds
     */
    IEnumerator ShowMessage(string message, float delay)
    {
        popText.text = message;
        popText.GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(delay);
        popText.GetComponent<Renderer>().enabled = false;
    }

    public void GetDamaged(int damage)
    {
        health = health - damage;
        StartCoroutine(ShowMessage("-" + damage, 3));
    }

    private void UpdateText()
    {
        if (currentPlayerTurn)
        {
            stats.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            stats.GetComponent<Renderer>().enabled = false;
        }
        stats.text = "HP: " + health.ToString() + "\nAP: " + curActionPoints.ToString();
    }

    public void UpdateState(bool state)
    {
        currentPlayerTurn = state;
        GetComponent<Renderer>().material.shader = Shader.Find("Specular");
        if (currentPlayerTurn)
        {
            this.GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {
            this.GetComponent<Renderer>().material.color = Color.red;
        }
    }

	// Update is called once per frame
	void Update()
    {
        if (health <= 0)
        {
            GetComponent<Renderer>().enabled = false;
            this.stats.GetComponent<Renderer>().enabled = false;
            this.tile.GetComponent<Tile>().isOccupied = false;
            tile.GetComponent<Tile>().state = 0;
            stats.GetComponent<Renderer>().enabled = false;
            Destroy(this);
        }
        UpdateText();
    }
}
