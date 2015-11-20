using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public string pName = "Name?";
	public int maxHealth = 100;
	public float pArmor = 0;
	public float runSpeed = 10;  
	public float floatSpeed = 5;  // This is how fast character can traverse in zero gravity (enhanced with jetpack/jetboots, etc
	public bool spaceWorthy = false; // This denotes if the character is equipped to be in free space
	public int attackPower = 1; // How much currently equipped player can hurt something (1 = fist)

	// Player's personal gear
	public Item pHat;
	public Item pPants;
	public Item pShirt;
	public Item pShoes;


	/* Contains the following functions:
	 * 1. Start()
	 * 3. Manage Player Gear (Allow drag and drop from inventory to player page)
	 * 4. AddPlayerGear(ItemType)
	 * 5. Show Player Screen
	 * 6. 
	 * */

	void Start()
	{
		/* 1. Set player to correct location (initial or saved)
		 * 2. Set player health to saved or initial health
		 * 3. Setup players gear and it's effects
		 * */
	}	






}
