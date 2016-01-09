/**********************************************
* Power Grid Inventory
* Copyright 2015 James Clark
**********************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PowerGridInventory.Extensions.ItemFilter
{
    /// <summary>
    /// Component used for handling item type filtering in a
    /// Grid Inventory equipment slot.
    /// </summary>
    [AddComponentMenu("Power Grid Inventory/Extensions/Item Filter/Item Type Filter")]
    [RequireComponent(typeof(PGISlot))]
    public class ItemTypeFilter : MonoBehaviour
    {
        public List<string> AllowedTypes;

        void Awake()
        {
            PGISlot slot = GetComponent<PGISlot>();
            if (slot != null)
            {
                slot.OnCanEquipItem.AddListener(CanEquip);
                slot.OnEquipItem.AddListener(OnEquip);
                slot.OnUnequipItem.AddListener(OnUnequip);
            }

        }

        public void CanEquip(PGISlotItem item, PGIModel inv, PGISlot slot)
        {
            //Debug.Log ("Can equip " + item.gameObject.name + " in " + inv.gameObject.name + " at slot " + slot.gameObject.name + "?");
            //filter out what can and can't be equipped
            if (AllowedTypes != null && AllowedTypes.Count > 0)
            {
                var type = item.GetComponent<ItemType>();
                if (type != null && AllowedTypes.Contains(type.TypeName))
                {
                    //Debug.Log ("\tYep.");
                    return;
                }

                //let the inventory know that things are not well
                inv.CanPerformAction = false;
            }
            //Debug.Log ("\tNope.");
        }

        public void OnEquip(PGISlotItem item, PGIModel inv, PGISlot slot)
        {
            Debug.Log(gameObject.name + " is recieving an item.");
        }

        public void OnUnequip(PGISlotItem item, PGIModel inv, PGISlot slot)
        {
            Debug.Log(gameObject.name + " is loosing an item.");
        }
    }
}
