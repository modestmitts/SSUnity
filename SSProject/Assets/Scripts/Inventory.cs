using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour 
{
	public GUISkin skin;

	public List<Item> inventory = new List<Item>();
	public List<Item> slots = new List<Item>();

	public bool showInventory = false;
	public bool showToolBelt = false;

	private ItemDatabase database;
	private int slotwidth = 42;
	private int slotheight = 42;
	private int slotseparation = 2;

	private bool showToolTip;
	private int tooltipHeight = 200;
	private int tooltipWidth = 200;
	private string tooltip;

	private int toolbeltSlots = 10;

	private int slotsX = 15;
	private int slotsY = 2;

	private bool draggingItem;
	private Item draggedItem;
	private int prevIndex; //The index of the now dragged item

	void Start()
	{
		for(int i = 0; i < (slotsX * slotsY + toolbeltSlots); i++)
		{
			slots.Add(new Item());
			inventory.Add (new Item());
		}

		database = GameObject.FindGameObjectWithTag("Item Database").GetComponent<ItemDatabase>();
		AddItem (0);
		AddItem (2);
		AddItem (1);
		AddItem (3);
			
	}

	void Update()
	{
        OnGUI();
	}

	void OnGUI()
	{
		tooltip = "";
		GUI.skin = skin;
		DrawInventory();
		if (showToolTip)
		{
			// If mouse position + 200 is below the bottom of the screen
			if ((Event.current.mousePosition.y + tooltipHeight) > Screen.height)
			{
				//draw tooltip above mouse
				GUI.Box (new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - tooltipHeight, tooltipWidth, tooltipHeight), tooltip, skin.GetStyle ("Tooltip"));
			}
			else 
			{
				GUI.Box (new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, tooltipWidth, tooltipHeight), tooltip, skin.GetStyle ("Tooltip"));
			}
		}

		if (draggingItem)
						GUI.DrawTexture (new Rect (Event.current.mousePosition.x, Event.current.mousePosition.y, 50, 50), draggedItem.itemIcon);
	}

	void DrawInventory()
	{
		Event e = Event.current;
		int i = 0; //List index number
		float xCenter = (Screen.width / 2) - ((slotwidth + slotseparation) * slotsX / 2);
		float xtCenter = (Screen.width / 2) - ((slotwidth + slotseparation) * toolbeltSlots / 2);
		float yBottom = (Screen.height); 
		int Num = 1;
		string SlotNumber;
	
		if (!showInventory)
			yBottom = Screen.height - (slotheight + slotseparation);
		else
			yBottom = Screen.height - ((slotsY + 1) * (slotheight + slotseparation));


		/* Drawing the Toolbelt first */
		for (int x = 0; x < toolbeltSlots; x++)
		{
			Num = Num % 10;
			SlotNumber = Num.ToString();

			// Draw the empty Toolbelt slot
			Rect slotRect = new Rect(x * (slotwidth + slotseparation) + xtCenter, yBottom, slotwidth, slotheight);
			GUI.Box(slotRect, SlotNumber, skin.GetStyle("Toolbelt")); 
			slots[i] = inventory[i];
			if (slots[i].itemName != null) //If there is something in the slot
			{
				// If there is somethign in the slot, draw the icon
				GUI.DrawTexture(slotRect, slots[i].itemIcon); // Draw Item Icon
				if (slotRect.Contains (e.mousePosition)) // checking to see if the mouse is over an item in the inventory
				{
					CreateToolTip(slots [i]);
					showToolTip = true;
					if (e.button == 0 && e.type == EventType.mouseDrag && !draggingItem ) //Are we dragging an item from a slot? (and not already dragging an item
					{
						draggingItem = true;
						prevIndex = i;
						draggedItem = slots[i];
						inventory[i] = new Item();
					}
					// Swap items if dragged item is a Handtool
					if (e.type == EventType.mouseUp && draggingItem && ((draggedItem.itemType == Item.ItemType.HandTool))) 
					{
						inventory[prevIndex] = inventory[i];
						inventory[i] = draggedItem;
						draggingItem = false;
						draggedItem = null;
					}
				}
			}
		
			else // The slot is empty			
			{
				if (slotRect.Contains (e.mousePosition)) //If the mouse is over the empty slot
				{
					// AND the mouse button is lifted AND we were dragging an item AND the item is a handtool then drop it in the slot
					if ((e.type == EventType.mouseUp && draggingItem) && (draggedItem.itemType == Item.ItemType.HandTool)) 
					{
						inventory[i] = draggedItem;
						draggingItem = false;
						draggedItem = null;
					}
				}
			}

			if (tooltip == "") 
			{
				showToolTip = false;
			}
			i++;
			Num++;
		}

		if (showInventory)
		{
			yBottom = Screen.height - (slotsY * (slotheight + slotseparation));
			for (int y = 0; y < slotsY; y++)
			{
				for (int x = 0 ; x < slotsX; x++)
				{
					Rect slotRect = new Rect(x * (slotwidth + slotseparation) + xCenter, yBottom + (y * (slotheight + slotseparation)), slotwidth, slotheight);
					GUI.Box(slotRect, "", skin.GetStyle("Slot")); 
					slots[i] = inventory[i];
					if (slots[i].itemName != null)			
					{
						GUI.DrawTexture(slotRect, slots[i].itemIcon); //Draw item Icon
						if (slotRect.Contains (e.mousePosition)) // checking to see if the mouse is over an item in the inventory
						{
							CreateToolTip(slots [i]);
							showToolTip = true;
							if (e.button == 0 && e.type == EventType.mouseDrag && !draggingItem) //Are we dragging an item from a slot? (and not already dragging an item
							{
								draggingItem = true;
								prevIndex = i;
								draggedItem = slots[i];
								inventory[i] = new Item();
							}

							// Swap slot with dragged unless slot item is not a handtool and would go into the toolbelt
							if ((e.type == EventType.mouseUp && draggingItem) && !((inventory[i].itemType != Item.ItemType.HandTool) && prevIndex < toolbeltSlots))
							{ 
								inventory[prevIndex] = inventory[i];
								inventory[i] = draggedItem;
								draggingItem = false;
								draggedItem = null;
							}
						}
					}
					else 					
					{
						if (slotRect.Contains (e.mousePosition))
						{
							if (e.type == EventType.mouseUp && draggingItem)
							{
								inventory[i] = draggedItem;
								draggingItem = false;
								draggedItem = null;
							}
						}
					}
					if (tooltip == "")
					{
						showToolTip = false;
					}
					i++;
					Num++;
				}
			}
		}
	}



	string CreateToolTip (Item item)
	{
		tooltip = "<color=#ffffff>" + item.itemName + "</color>\n\n" + item.itemDesc; // Tool tip name and font color
		return tooltip;
	}


void AddItem(int id)
{
	bool isHandTool = false;
	for (int j = 0; j < database.items.Count; j++) // Go through the database
	{
		if (database.items[j].itemID == id) // Find the item in the database
		{
			if (database.items[j].itemType == Item.ItemType.HandTool) //Is item a Handtool?
			{
				isHandTool = true;
			}
			for (int i = 0; i < inventory.Count; i++) // Go through the inventory list
			{
				//If there is an empty slot AND the item is a handtool in the toolbelt slots OR the slot is regular inventory
				if ((inventory[i].itemName == null) && (((isHandTool && (i < toolbeltSlots)) || (i >= toolbeltSlots))))
					{
						inventory[i] = database.items[j];
						break;
					}
			}
		}
	}
}

bool InventoryContains(int id)
{
	bool result = false;
	for (int i = 0; i < inventory.Count; i++) 
	{
		result = inventory[i].itemID == id;
		if (result) break;
	}
	return result;
}

void RemoveFromInventory(int id)
{
	for (int i = 0; i < inventory.Count; i++) 
	{
		if (inventory[i].itemID == id)
		{
			inventory[i] = new Item();
			break;
		}
	}
}



	/*
	void RemoveItemFromSlot(int slotnum)
	{

	}*/


}