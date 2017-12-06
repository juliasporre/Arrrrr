using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour {
    public GameObject tile;
    public GameObject weaponPrefab;
    public GameObject statsMesh;
    public GameObject popMesh;

    public int indexNumber;

    public int iniActionPoints;
    public int curActionPoints;

    public int atkRange = 0;
    public int health = 15;
    public int damage = 0;

    public bool currentPlayerTurn = false;
    public bool hasAttacked = false;
    public bool hasTreasure = false;
    public bool inFOW = false;
    public int state = 0;
    public int ownerNumber = -1;

    private int searchTimer = 0;

    public TextMesh stats;
    public TextMesh popText;
    public GameObject bgPrefab;
    public GameObject statsBG;

    public List<GameObject> weaponsList = new List<GameObject>();
    public int weaponNumbers = 2;
    // Use this for initialization
    void Start ()
    {
        //gameObject.name = "Ship " + indexNumber.ToString();
        curActionPoints = iniActionPoints;

        var statsClone = Instantiate(statsMesh, transform);
        stats = statsClone.GetComponent<TextMesh>();
        stats.text = "HP: " + health.ToString() + "\nAP: " + curActionPoints.ToString();
        stats.GetComponent<Renderer>().enabled = false;

        var statsBGC = Instantiate(bgPrefab, transform);
        statsBG = statsBGC;
        statsBG.GetComponent<Renderer>().enabled = true;

        var popClone = Instantiate(popMesh, transform);
        popText = popClone.GetComponent<TextMesh>();
        popText.GetComponent<Renderer>().enabled = false;
        
        CreateWeapons();
    }

    void CreateWeapons()
    {
        for (int weaponsCreated = 0; weaponsCreated < weaponNumbers; weaponsCreated++)
        {
            var weaponsClone = Instantiate(weaponPrefab, transform);
            weaponsClone.name = "Weapon " + weaponsCreated.ToString();
            weaponsClone.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            if (weaponsCreated == 0)
            { 
                weaponsClone.GetComponent<Weapon>().damage = 5;
                weaponsClone.GetComponent<Weapon>().range = 1;
            }
            else
            {
                weaponsClone.GetComponent<Weapon>().damage = 1;
                weaponsClone.GetComponent<Weapon>().range = 4;
            }
            weaponsList.Add(weaponsClone);
        }
        return;
    }

    public void ChangeWeapons(int option)
    {
        damage = weaponsList[option].GetComponent<Weapon>().damage;
        atkRange = weaponsList[option].GetComponent<Weapon>().range;
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
            //popText.GetComponent<Renderer>().enabled = false;
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
    
    private void UpdateFOW()
    {
        var ggen = GameObject.Find("Grid").GetComponent<GridGenerator>();
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        if (ggen.myPlayerNumber != ownerNumber && !inFOW)
        {
            Debug.Log("Enemy ship not in fow, turning of meshRenderer");
            this.GetComponent<Renderer>().enabled = false;
            foreach (Renderer r in rs)
            {
                r.enabled = false;
            }
        }
        else
        {
            this.GetComponent<Renderer>().enabled = true;
            foreach (Renderer r in rs)
            {
                r.enabled = true;
            }
        }
        return;
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
            return false;
        }
        return true;
    }

	// Update is called once per frame
	void Update()
    {
        UpdateText();
        UpdateFOW();
        foreach (GameObject wea in weaponsList)
        {
            wea.GetComponent<Renderer>().enabled = false;
        }
    }
}
