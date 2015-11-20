using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Generics gives access to list
// Use List type, which is dynamic and length does not need to be pre-defined

public class ItemDatabase : MonoBehaviour
{
	public List<Item>  items = new List<Item>(); 

	void Start()
	{
		items.Add (new Item ("Small Liquid AM Fuel Suspension Tank", 0, "Liquid Thorium holding tank for AM drives", 450, 10, 2500, Item.ItemType.EngineFuelProc, Item.ItemAttType.Engine_AntiMatter,0, 0, 0, 0));
		items.Add (new Item ("Small Anti-Matter Drive", 1, "An antimatter engine used to power small ship.", 1500, 220, 300, Item.ItemType.EnginePowerGen, Item.ItemAttType.Engine_AntiMatter,10, 10, 0, 0));
		items.Add (new Item ("Small Standard AM Gyro Mount Thruster", 2, "Small thruster for propulsion on Anti-Matter craft.", 600, 25, 5, Item.ItemType.EngineVectControl, Item.ItemAttType.Engine_AntiMatter,1, 0, 0, 0));
		items.Add (new Item ("Simple Electric Screwdriver", 3, "Small rechargable electric screwdriver. You have to change the tips manually.", 10, 1, 1, Item.ItemType.HandTool, Item.ItemAttType.Tool_ElectricScrewdriver, 1, 0, 0, 0)); 
		items.Add (new Item ("Etched Ultra-Efficient Small Anti-Matter Drive", 4, "Very Efficient Small Anti-Matter Drive", 1800, 220, 300, Item.ItemType.EnginePowerGen, Item.ItemAttType.Engine_AntiMatter,10, 15, 0, 0));

	}
	
}

