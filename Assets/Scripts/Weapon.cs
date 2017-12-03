using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public int damage = 5;
    public int range = 5;

    private void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void Update()
    {
    
    }
}