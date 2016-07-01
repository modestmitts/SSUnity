using UnityEngine;
using System.Collections;
using PowerGridInventory;

public class MyTrashcan : MonoBehaviour 
{
   
    private Transform t;
    private Vector3 playerPos;
    public void TrashItem(PGISlotItem item, PGIModel inv, PGISlot slot)
    {
        playerPos = GameObject.Find("Player").transform.position;
         t = item.gameObject.transform;
         print("Player transform X: " + playerPos.x + " Y: " + playerPos.y);
    
        t.position = playerPos + Vector3.forward + Vector3.one;

        inv.Drop(item);
    }
  
    

}
