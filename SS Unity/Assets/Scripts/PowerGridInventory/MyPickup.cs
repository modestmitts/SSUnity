using UnityEngine;
using System.Collections;
using PowerGridInventory;

[RequireComponent (typeof (PGISlotItem))]
[RequireComponent(typeof(BoxCollider))]

public class MyPickup : MonoBehaviour 
{
    public PGIModel DefaultInventory;
    private PGISlotItem Item;
    private PartDetach pd;

    void Awake()
    {
        Item = GetComponent<PGISlotItem>();
        pd = GetComponent<PartDetach>();
    }

    
    void OnMouseDown()
    {
        if (pd == null)
        { 
           if (DefaultInventory != null && Item != null)
               DefaultInventory.Pickup(Item);
        }

        else if (DefaultInventory != null && Item != null && pd.Detached)
            DefaultInventory.Pickup(Item);
    }
     

    public void RaycastPickup()
    {
        if (pd == null) // 
        { 
           if (DefaultInventory != null && Item != null)
               DefaultInventory.Pickup(Item);
        }

            // Is the inventory able to take item and is it detached?
        else if (DefaultInventory != null && Item != null && pd.Detached)
            DefaultInventory.Pickup(Item);
    }
 
}
