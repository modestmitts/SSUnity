﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerGridInventory;

public class MyItemFilter : MonoBehaviour 
{
    public List<string> AllowedTypes;

    public void IsTypeValid(PGISlotItem item, PGIModel model, PGISlot slot)
    {
        print("IN IsTypeValid");
        if (AllowedTypes != null && AllowedTypes.Count > 0)
        {
            
            model.CanPerformAction = true;

            var type = item.GetComponent<MyItemType>();
            if (type != null && AllowedTypes.Contains(type.Type))
            {
                print("In the good return");
                return;
            }

            //Let the inventory know that things are not well
            model.CanPerformAction = false;
        }

    }
}