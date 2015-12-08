/**********************************************
* Power Grid Inventory
* Copyright 2015 James Clark
**********************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using PowerGridInventory.Utility;

namespace PowerGridInventory
{
    /// <summary>
    /// This component allows a <see cref="PGISlotItem"/> to become a valid
    /// drop target in the grid for other socketable items.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Power Grid Inventory/Socketed", 15)]
    [RequireComponent(typeof(PGISlotItem))]
    [Serializable]
    public class Socketed : MonoBehaviour
    {
        #region Members
        [Serializable]public class SocketEvent : UnityEvent<PGIModel, Socketed, Socketable> { };

        /// <summary>
        /// A filtering identifier that must correspond to any incoming socketable item ids in order for the socketing to be allowed.
        /// </summary>
        [Tooltip("A filtering identifier that must correspond to any incoming socketable item ids in order for the socketing to be allowed.")]
        public int SocketId = 0;

        /// <summary>
        /// Wether or not this item should propogate its PGI events (store, equip, drop, unequip) to the items that are socketed to it.
        /// If not set, socketed item will never receive any of these events for any inventory actions taken with this item.
        /// </summary>
        [Tooltip("Wether or not this item should propogate its PGI events (store, equip, drop, unequip) to the items that are socketed to it. If not set, socketed item will never receive any of these events for any inventory actions taken with this item.")]
        public bool PropogateEvents = false;

        /// <summary>
        /// The list of sockets and their contents. Null entries can exists and represent empty sockets.
        /// </summary>
        [Tooltip("The list of sockets and their contents. Null entries can exists and represent empty sockets.")]
        public Socketable[] Sockets;

        /// <summary>
        /// Invoked when a socketable <see cref="PGISlotItem"/> is about to be dropped into
        /// a socketed one. You can disallow this action by setting the the provided model's
        /// <see cref="PGIModel.CanPerformAction"/> to <c>false</c>.
        /// </summary>
        [SerializeField] [PGIFoldedEvent] public SocketEvent OnCanSocket = new SocketEvent();

        /// <summary>
        /// Invoked after a socketable <see cref="PGISlotItem"/> has been dropped and
        /// attached to a socketed one.
        /// </summary>
        [SerializeField] [PGIFoldedEvent] public SocketEvent OnSocketed = new SocketEvent();
        [SerializeField] [PGIFoldFlag] public bool FoldedEvents = false; //used by the inspector

        /// <summary>
        /// Returns the number of un-used sockets.
        /// </summary>
        public int EmptySockets
        {
            get
            {
                if (Sockets == null || Sockets.Length < 1) return 0;

                int count = 0;
                foreach (var soc in Sockets)
                {
                    if (soc != null) count++;
                }
                return count;
            }
        }
        #endregion


        #region Methods
        /// <summary>
        /// Returns the index to the first socket that is not used or -1 if there are none.
        /// </summary>
        /// <returns></returns>
        public int GetFirstEmptySocket()
        {
            if (Sockets == null || Sockets.Length < 1) return -1;

            for (int i = 0; i < Sockets.Length; i++)
            {
                if (Sockets[i] == null) return i;
            }

            return -1;
        }

        /// <summary>
        /// Attempts to attach one socketable item to this socketed one.
        /// </summary>
        /// <param name="receiver">The socketed item that will receive the other.</param>
        /// <param name="thing">The socketable item that will be attached.</param>
        /// <returns>The index of the socket array the socketable was stored in, or -1 if it was not stored.</returns>
        public int AttachSocketable(Socketable thing)
        {
            if (thing == null) return -1;
            if (!thing.SocketId.Equals(SocketId)) return -1;
            var index = GetFirstEmptySocket();
            if (index != -1)
            {
                Sockets[index] = thing;
                return index;
            }
            return -1;
        }

        /// <summary>
        /// Attempts tp remove one socketable item from this socketed one.
        /// </summary>
        /// <param name="thing">The socketable item being removed.</param>
        /// <returns><c>true</c> if the item was removed, <c>false</c> otherwise.</returns>
        public bool DetachSocketable(Socketable thing)
        {
            if (thing == null) return false;
            for (int i = 0; i < Sockets.Length; i++)
            {
                if (Sockets[i] == thing)
                {
                    Sockets[i] = null;
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}
