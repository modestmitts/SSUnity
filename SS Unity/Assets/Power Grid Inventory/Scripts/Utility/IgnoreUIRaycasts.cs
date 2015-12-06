/**********************************************
* Power Grid Inventory
* Copyright 2015 James Clark
**********************************************/
using UnityEngine;
using System.Collections;

namespace PowerGridInventory.Utility
{
    /// <summary>
    /// Causes the uGUI elemement to Ignore user interface raycasts
    /// when active. Useful for elements that will appear near the mouse
    /// pointer but should not block controls that are undernear.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Power Grid Inventory/Utility/Ignore Raycasts", 21)]
    public class IgnoreUIRaycasts : MonoBehaviour, ICanvasRaycastFilter
    {
        void Start()
        {
        }

        /// <summary>
        /// Used to filter this objct out of UI raycasts.
        /// </summary>
        /// <returns><c>true</c> if this component is active, otherwise <c>false</c>.</returns>
        /// <param name="sp">Sp.</param>
        /// <param name="eventCamera">Event camera.</param>
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return !gameObject.activeSelf;
        }
    }
}
