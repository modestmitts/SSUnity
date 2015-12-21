using UnityEngine;
using System.Collections;
using PowerGridInventory;

public class MyTrashcan : MonoBehaviour 
{
    public void TrashItem(PGISlotItem item, PGIModel inv, PGISlot slot)
    {
        //First, remove this item from the equipement slot
        inv.Unequip(item);

        //Next, trigger removal events on the item.
        item.TriggerCanUnequipEvents(inv, slot);
        item.TriggerRemoveEvents(inv);
    }
  
    

}
