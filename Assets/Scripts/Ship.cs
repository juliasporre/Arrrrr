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
    public bool hasAttacked = false;
    public bool hasTreasure = false;
    public int state = 0;

    private int searchTimer = 0;

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

    /*
     * Update the HP and AP indicators next to ships. This also acts as Island Search indicator
     */
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
        if (state == 3)
        {
            stats.text = "Searching Island\n" + searchTimer + " Turns Left";
        }
    }

    /*
     * Update Materials. If enemy it's red, if ally it's blue
     */
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

    /*
     * Decrease timer. Called in SetTurn
     */
    public void UpdateTimer()
    {
        if (searchTimer > 0)
        {
            searchTimer--;
        }
        if (state != 0 && searchTimer == 0)
        {
            SetState(0);
        }
    }
    /*
     * Set state for ship (currently only 3). If newState is 3, activate a timer
     */
    public void SetState(int newState)
    {
        state = newState;
        if (newState == 3)
        {
            hasTreasure = true;
            searchTimer = 3;
        }
    }

    /*
     * Check health of ship. This is called in the Player Update().
     */
    public bool CheckHealth()
    {
        if (health <= 0)
        {
            var tilescript = tile.GetComponent<Tile>();
            stats.GetComponent<Renderer>().enabled = false;
            tilescript.isOccupied = false;
            tilescript.occuObject = null;
            tilescript.state = 0;
            GetComponent<Renderer>().enabled = false;
            Destroy(this);
            return false;
        }
        return true;
    }

	// Update is called once per frame
	void Update()
    {
        UpdateText();
    }
}
