/*********************************************	
 *               ShipClass
This is the top level object under which all other 
objects reside … kinda. When things are removed or 
added to the ship, they take their whole tree with
them. 

ShipClass-> <LIST>StructureClass-> <LIST> PanelClass -->
<LIST> ComponentClass  
*********************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ShipClass : MonoBehaviour 
{
    /*
    public string shipName; // This is the name of the individual ship
    public string shipModel; // This is the kind of ship it is 
    public bool isSecure; //Whether the ship's console is locked or not
    public string passCode; //If Secure, this is the passcode to get in
    public ShipDataClass ShipData; // This is how the ship performs/functions with current enabled structures/components. 
    public List <Structure> structures;

// Prefab? Model? Pics? Some kind of way to link to the actual 3D thing?

    public ShipClass()
    {
    }

    public ShipClass(string sName, string sModel, bool iS, string pC, ShipDataClass SDC, List<Structure> structure)
    {
	    shipName = sName;
	    shipModel = sModel;
	    isSecure = iS;
	    passCode = pC;
	    ShipData = SDC;
	    structures = structure;
	    // Models, Pics?
    }
    */
}

