﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ShipData : MonoBehaviour 
{
    public string shipOwner; //Owner of the ship
    public string shipTitle; // Name of the ship or Model
    public string shipID; // Numeric Identifier
    public Sprite shipIcon; // Owners Icon
    public string shipSecStatus; //
    public string shipOverMsg; //
    public string password; // NA if none
    // Ship Access Stamp List (Not sure if needed)
    public List<GameObject> panelList;// Hatch and Door List (Structs denoting locked and open status) 

    /* Build initial ship function
     * Gathers all the panels, hatches and doors as a list
     */
    void Start()
    {
        CollectDoors();
    }

    public void CollectDoors()
    {
        Transform Rent = transform.parent;

        foreach (Transform child in Rent)
        {
            print("Child name: " + child.gameObject.name);
            if (child.tag == "panel1")
            {
                panelList.Add(child.gameObject);
            }
        }
    }


}
