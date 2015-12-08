using UnityEngine;
using System.Collections;
using PowerGridInventory;

[RequireComponent (typeof (PGISlotItem))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class MyPickup : MonoBehaviour 
{
    public PGIModel DefaultInventory;
    private PGISlotItem Item;

    void Awake()
    {
        Item = GetComponent<PGISlotItem>();
    }

    void OnMouseDown()
    {
        if (DefaultInventory != null && Item != null)
            DefaultInventory.Pickup(Item);
    }
 
}
