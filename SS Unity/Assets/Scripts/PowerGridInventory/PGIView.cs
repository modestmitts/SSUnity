/**********************************************
* Power Grid Inventory
* Copyright 2015 James Clark
**********************************************/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PowerGridInventory;
using System.Collections;
using PowerGridInventory.Utility;
using AncientCraftGames.UI;

namespace PowerGridInventory
{
    /// <summary>
    /// Provies the corresponding UI view for a PGIModel.
    /// This particulatr view allows pointer manipulation with 
    /// click-and-hold Drag n' Drop funcitonality.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [AddComponentMenu("Power Grid Inventory/PGI Inventory View", 12)]
    [RequireComponent(typeof(RectTransform))]
    public class PGIView : MonoBehaviour
    {
        #region Members
        public static DragIcon DragIcon;

        public enum HorizontalOrdering
        {
            LeftToRight,
            RightToLeft,
        }
        public enum VerticalOrdering
        {
            TopToBottom,
            BottomToTop,
        }

        /// <summary>
        /// The prefab that will be used when generating this view's grid. This prefab must follow the guidlines specified in the manual under 'Slot Prefab Spec'.
        /// </summary>
        //[Header("Prefabs")]
        [Tooltip("The prefab that will be used when generating this view's grid. This prefab must follow the guidlines specified in the manual under 'Slot Prefab Spec'.")]
        public GameObject SlotPrefab;

        
        [HideInInspector]
        [SerializeField]
        private PGIModel _Model;
        /// <summary>
        /// The <see cref="PGIModel"/> whose data is displayed and manipulated by this view.
        /// </summary>
        [HideInInspector]
        public PGIModel Model
        {
            get
            {
                return _Model;
            }
            set
            {
                if(value != _Model)
                {
                    ClearList.Remove(this);
                    //out with the old
                    if (_Model != null)
                    {
                        _Model.OnBeginGridResize.RemoveListener(ResetGridSlots);
                        _Model.OnEndGridResize.RemoveListener(CreateGrid);
                        _Model.OnUpdateDirty -= this.UpdateViewStep;
                    }

                    //in with the new
                    _Model = value;
                    CreateGrid();
                    //SetupEquipment();

                    if (_Model != null)
                    {
                        _Model.OnBeginGridResize.AddListener(ResetGridSlots);
                        _Model.OnEndGridResize.AddListener(CreateGrid);
                        _Model.OnUpdateDirty += this.UpdateViewStep;
                    }

                    this.UpdateViewStep();
                }
                else _Model = value;
            }
        }

        /// <summary>
        /// Disables the ability to begin dragging items from this PGIView. Hover and click functionalities are not affected by this.
        /// </summary>
        [Header("Drag & Drop Toggles")]
        [Tooltip("Disables the ability to begin dragging items from this PGIView. Hover and click functionalities are not affected by this.")]
        public bool DisableDragging;

        /// <summary>
        /// Disables the ability to drop items from this inventory or others into this PGIView's associated model.
        /// </summary>
        [Tooltip("Disables the ability to drop items from this inventory or others into this PGIView's associated model.")]
        public bool DisableDropping;

        /// <summary>
        /// Disables the ability to remove items from the model by dragging them outside of the view's confines.
        /// </summary>
        [Tooltip("Disables the ability to remove items from the model by dragging them outside of the view's confines.")]
        public bool DisableWorldDropping;

        /// <summary>
        /// This allows the view to re-arrange the child objects for all of this view's grid Slots in order to aid uGUI's batching process.
        /// Keeping this on will usually decrease draw calls and increase performance but may introduce rendering artifacts depending
        /// on the setup of your slot prefab.
        /// </summary>
        [Header("Grid Behaviour")]
        [Tooltip("This allows the view to re-arrange the child objects for all of this view's grid Slots in order to aid uGUI's batching process. Keeping this on will usually decrease draw calls and increase performance but may introduce rendering artifacts depending on the setup of your slot prefab.")]
        public bool BatchSlots = true;

        /// <summary>
        /// The column order items will be inserted into the grid view.
        /// </summary>
        [Tooltip("The column order the grid will be created.")]
        public HorizontalOrdering HorizontalOrder = HorizontalOrdering.LeftToRight;

        /// <summary>
        /// The row order items will be inserted into the grid view.
        /// </summary>
        [Tooltip("The row order the grid will be created.")]
        public VerticalOrdering VerticalOrder = VerticalOrdering.TopToBottom;

        /// <summary>
        /// The color used in the 'highlight' section of grid and equipment slots when no action is being taken.
        /// </summary>
        [Header("Slot Colors")]
        [Tooltip("The color used in the 'highlight' section of grid and equipment slots when no action is being taken.")]
        public Color NormalColor = Color.clear;

        /// <summary>
        /// The color used in the 'highlight' section of grid and equipment slots when a valid action is about to occur.
        /// </summary>
        [Tooltip("The color used in the 'highlight' section of grid and equipment slots when a valid action is about to occur.")]
        public Color HighlightColor = Color.green;

        /// <summary>
        /// The color used in the 'hilight' section of a grid and equipment slots when a valid socket action is about to occur.
        /// </summary>
        [Tooltip("The color used in the 'hilight' section of a grid and equipment slots when a valid socket action is about to occur.")]
        public Color SocketValidColor = Color.green;

        /// <summary>
        /// The color used in the 'highlight' section of grid and equipment slots when an invalid action is being taken.
        /// </summary>
        [Tooltip("The color used in the 'highlight' section of grid and equipment slots when an invalid action is being taken.")]
        public Color InvalidColor = Color.red;

        /// <summary>
        /// The color used in the 'highlight' section of grid and equipment slots when it has been flagged as bocked with the <see cref="PGISlot.Blocked"/> value.
        /// </summary>
        [Tooltip("The color used in the 'highlight' section of grid and equipment slots when it has been flagged as bocked with the PGISlot.Blocked value.")]
        public Color BlockedColor = Color.grey;


        /// <summary>
        /// Invoked when the pointer first enters equipment slot or grid location with an item in it.
        /// <seealso cref="PGISlot.OnHover"/> 
        /// </summary>
        [SerializeField]
        [PGIFoldedEvent]
        public PGISlot.SlotTrigger OnHoverSlot = new PGISlot.SlotTrigger();

        /// <summary>
        /// Invoked when the pointer leaves an equipment slot or grid location with an item in it.
        /// <seealso cref="PGISlot.OnEndHover"/> 
        /// </summary>
        [SerializeField]
        [PGIFoldedEvent]
        public PGISlot.SlotTrigger OnEndHoverSlot = new PGISlot.SlotTrigger();

        /// <summary>
        /// Invoked when the pointer is clicked on an equipment slot or grid location with an item in it.
        /// <seealso cref="PGISlot.OnClick"/> 
        /// <seealso cref="PGISlotItem.OnClick"/> 
        /// </summary>
        [SerializeField]
        [PGIFoldedEvent]
        public PGISlot.SlotTrigger OnClickSlot = new PGISlot.SlotTrigger();

        /// <summary>
        /// Invoked when a drag operation begins on an equipment slot or grid location with an item in it.
        /// <seealso cref="PGISlot.OnDragEnd"/> 
        /// <seealso cref="PGISlotItem.OnClick"/> 
        /// </summary>
        [SerializeField]
        [PGIFoldedEvent]
        public PGISlot.SlotTrigger OnSlotDragBegin = new PGISlot.SlotTrigger();

        /// <summary>
        /// Invoked when a previous started drag operation ends.
        /// <seealso cref="PGISlot.OnDragBegin"/> 
        /// <seealso cref="PGISlotItem.OnClick"/> 
        /// </summary>
        [SerializeField]
        [PGIFoldedEvent]
        public PGISlot.SlotTrigger OnSlotDragEnd = new PGISlot.SlotTrigger();


        [SerializeField]
        [PGIFoldFlag]
        public bool FoldedEvents = false; //used by the inspector

        /// <summary>
        /// Returns true if this view is currently in a drag operational state.
        /// </summary>
        public bool IsDragging { get { return (DraggedItem == null); } }
        private Canvas UICanvas;				//canvas that renders the grid
        //This field needs to be serialized so that we can properly remove any GridSlots
        //that were attached during edit mode by traversing this list.
        [SerializeField]
        private List<PGISlot> Slots = new List<PGISlot>(5);
        private RectTransform ParentRect;
        private Coroutine UpdateViewCoroutine;
        private float CellScaleX = -1;
        private float CellScaleY = -1;
        public static CachedItem DraggedItem;
        private static List<PGIView> ClearList = new List<PGIView>(5);

        /// <summary>
        /// Child UI elements of slots are moved to this array of  GameObjects
        /// to allow UGui to batch them and reduce draw calls.
        /// </summary>
        protected GameObject[] SlotBatches;


#if UNITY_EDITOR
        //used to tell if we need to re-render in edit mode
        private int CachedSizeX;
        private int CachedSizeY;
        private float CachedRectX;
        private float CachedRectY;

#endif

        /// <summary>
        /// Helper class for managing items being moved internally due to drag n' drop actions.
        /// </summary>
        public class CachedItem
        {
            public PGISlotItem Item;
            public int xPos, yPos, Width, Height, EquipIndex;
            public PGISlot Slot;
            public PGIModel Model;
            public PGIView View;
            public bool WasEquipped { get { return (EquipIndex >= 0); } }
            public bool WasStored { get { return (xPos >= 0 && yPos >= 0); } }

            public CachedItem(PGISlotItem item, PGISlot slot, PGIModel model, PGIView view)
            {
                Item = item;
                xPos = item.xInvPos;
                yPos = item.yInvPos;
                Width = item.CellWidth;
                Height = item.CellHeight;
                EquipIndex = item.Equipped;
                Slot = slot;
                Model = model;
                View = view;
            }
        }
        #endregion


        #region Unity Events
        void Awake()
        {
            if (Model == null)
            {
                Model = GetComponentInChildren<PGIModel>();
                if (Model == null) Model = GetComponentInParent<PGIModel>();
                if (Model == null)
                {
                    Debug.LogError("Missing inventory backend.");
                }
            }
            UICanvas = gameObject.GetComponentInParent<Canvas>();
            if (UICanvas == null)
            {
                throw new UnityException("There must be a parent of this element that has a Canvas component attached.");
            }
            if (DragIcon == null && Application.isPlaying) DragIcon = CreateDragIcon(UICanvas.transform);
        }

        static DragIcon CreateDragIcon(Transform dragParent)
        {
            GameObject di = new GameObject("Drag Icon");
            di.AddComponent<RectTransform>();
            DragIcon dragIcon = di.AddComponent<DragIcon>();
            di.transform.SetParent(dragParent);
            di.transform.SetSiblingIndex(dragParent.childCount - 1);
            di.transform.localScale = Vector3.one;
            
            GameObject icon = new GameObject("Icon");
            icon.transform.SetParent(di.transform);
            var rect = icon.AddComponent<RectTransform>();
            icon.AddComponent<CanvasRenderer>();
            icon.AddComponent<Image>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;

            GameObject icon3d = new GameObject("Icon3D");
            icon3d.transform.SetParent(di.transform);
            rect = icon3d.AddComponent<RectTransform>();
            icon3d.AddComponent<CanvasRenderer>();
            icon3d.AddComponent<Image3D>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;

            dragIcon.Icon = icon.GetComponent<Image>();
            dragIcon.Icon3D = icon3d.GetComponent<Image3D>();
            dragIcon.gameObject.SetActive(false);
            return dragIcon;

        }

        void Start()
        {
            CreateGrid();
            SetupEquipment();

            if (Model != null)
            {
                Model.OnBeginGridResize.AddListener(ResetGridSlots);
                Model.OnEndGridResize.AddListener(CreateGrid);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {

            if (!Application.isPlaying && Model != null)
            {
                if (this.CachedSizeX != Model.GridCellsX || this.CachedSizeY != Model.GridCellsY ||
                    this.CachedRectX != ParentRect.rect.width || this.CachedRectY != ParentRect.rect.height)
                {
                    //if we are editing, re-create the grid every time we enable
                    CreateGrid();
                    SetupEquipment();
                }
            }

        }
#endif
        void OnEnable()
        {
            if (_Model != null) _Model.OnUpdateDirty += this.UpdateViewStep;
        }

        void OnDisable()
        {
            //We add a small delay so that if triggered events attached to the view was the one that disabled it, they have time to
            //handle everything before we actually disable the view. This is particularly important to the
            //InventoryItem class. When the item is dropped, the nested inventory view must be closed, but doing
            //so causes the item to be returned to its own inventory due to the
            //shared nature of the DraggedItem. This little delay helps avoid that scenario.
            this.Invoke("DisableView", 0.05f);

            if (_Model != null) _Model.OnUpdateDirty -= this.UpdateViewStep;
        }

        IEnumerator DisableView()
        {
            //Usually happens when we drag an empty slot
            if (DraggedItem == null)
            {
                OnSlotDragEnd.Invoke(null, null);
                yield return null;
            }
            else
            {
                OnSlotDragEnd.Invoke(null, DraggedItem.Slot);
                DraggedItem.Model.ResetSwapCache();
            }

            ReturnDraggedItemToSlot();
            Model.ResetSwapCache();
            DeselectAllViews();
            DraggedItem = null;
            DragIcon.gameObject.SetActive(false);
            ResetDragIcon(DragIcon);
            yield return null;
        }

        /// <summary>
        /// Completely refreshes the view's grid and all equipment slots of the view's model.
        /// Internally, slots will be re-sized and have their highlighting and icons set apporpriately
        /// according to slot contents.
        /// </summary>
        public void UpdateViewStep()
        {
            if (Model == null) return;
            UpdateDirtyGrid();

            //TODO: We need to make equipment slots part of the overall dirty-flag of the model
            //if any equip slots are dirty, simply update the highlighting
            foreach (PGISlot slot in Model.GetAllDirtySlots())
            {
                if (slot.Blocked) slot.HighlightColor = BlockedColor;
                slot.AssignItem(slot.Item);
                slot.Dirty = false;
            }
            
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Utility for resetting all slots to default state. Used internally for resizing grids.
        /// </summary>
        void ResetGridSlots()
        {
            ResetSlotRange(0, 0, Model.GridCellsX, Model.GridCellsY);
        }

        /// <summary>
        /// Creates a grid of <see cref="PGISlot"/>s and sets up
        /// all events and references used by the view and model.
        /// </summary>
        void CreateGrid()
        {
            if (Model == null) return;
            ParentRect = this.GetComponent<RectTransform>();
            CellScaleX = ParentRect.rect.width / Model.GridCellsX;
            CellScaleY = ParentRect.rect.height / Model.GridCellsY;
            foreach (PGISlot slot in Slots)
            {
                if (slot != null)
                {
                    if (Application.isPlaying) GameObject.Destroy(slot.gameObject);
                    else GameObject.DestroyImmediate(slot.gameObject);
                }
            }
            Slots.Clear();

            for (int y = 0; y < Model.GridCellsY; y++)
            {
                for (int x = 0; x < Model.GridCellsX; x++)
                {
                    //initialize slot
                    GameObject slotGO = GameObject.Instantiate(SlotPrefab, Vector3.zero, new Quaternion()) as GameObject;
                    PGISlot slot = slotGO.GetComponent<PGISlot>();
                    Slots.Add(slot);
                    slotGO.transform.position = Vector3.zero;
                    slotGO.transform.SetParent(this.transform, false);
                    slotGO.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    slotGO.name = "Slot " + x + "," + y;

                    //EDIT: Took this out. Slots now reference mod through their view reference
                    //slot.Model = Model;
                    slot.View = this;
                    slot.xPos = x;
                    slot.yPos = y;
                    slot.HighlightColor = NormalColor;

                    //position and size
                    RectTransform childRect = slotGO.GetComponent<RectTransform>();
                    childRect.anchoredPosition = CalculateCellPos(x, y);
                    childRect.sizeDelta = new Vector2(CellScaleX, CellScaleY);

                    //setup events
                    slot.OnBeginDragEvent.AddListener(OnDragBegin);
                    slot.OnEndDragEvent.AddListener(OnDragEnd);
                    slot.OnDragEvent.AddListener(OnDrag);
                    slot.OnHover.AddListener(OnHoverEvent);
                    slot.OnEndHover.AddListener(OnEndHoverEvent);
                    slot.OnClick.AddListener(OnClickSlotHandler);

                    slot.Dirty = true;
                }
            }

#if UNITY_EDITOR
            CachedSizeX = Model.GridCellsX;
            CachedSizeY = Model.GridCellsY;
            CachedRectX = ParentRect.rect.width;
            CachedRectY = ParentRect.rect.height;
#endif

            //This is a performance trick. When running in-game, we will
            //take all of the child elements of our slots and parent them
            //under a single gameobject held by this view. In this way we
            //can allow UGui batch more UI elements and reduce draw calls.
            if (Application.isPlaying && BatchSlots)
            {
                PerformSlotBatching(ref SlotBatches, Slots.ToArray(), this.transform, UICanvas, true);
            }
        }

        /// <summary>
        /// Sets up all references and triggered events for this view's model's Equipment slots.
        /// </summary>
        void SetupEquipment()
        {
            if (Model == null) return;
            if (Model.Equipment != null)
            {
                for (int i = 0; i < Model.Equipment.Length; i++)
                {
                    var slot = Model.Equipment[i];
                    if (slot != null)
                    {
                        //NOTE: These top two aren't needed here now. It's done in the model's Start() method
                        //slot.EquipmentIndex = i;
                        //slot.Model = Backend;
                        slot.View = this;
                        slot.OnBeginDragEvent.AddListener(OnDragBegin);
                        slot.OnEndDragEvent.AddListener(OnDragEnd);
                        slot.OnDragEvent.AddListener(OnDrag);
                        slot.OnHover.AddListener(OnHoverEvent);
                        slot.OnEndHover.AddListener(OnEndHoverEvent);
                        slot.HighlightColor = NormalColor;
                        //slot.Highlight.gameObject.SetActive(false);
                    }
                }
            }

            if (Application.isPlaying && BatchSlots)
            {
                PerformSlotBatching(ref SlotBatches,
                    Model.Equipment,
                    this.transform,
                    UICanvas,
                    (Model.GridCellsX > 0 && Model.GridCellsY > 0) ? false : true); //pass false if we have a grid since already re-created them then
            }
        }

        /// <summary>
        /// Used when creating a grid to help arrange child object of slots
        /// to be more easily batched by UGui's renderer.
        /// </summary>
        /// <returns></returns>
        /// <param name="batches">A reference to an array of GameObject that represent each batch.</param>
        /// <param name="slots">An array of PGISlots that will be batched.</param>
        static void PerformSlotBatching(ref GameObject[] batches, PGISlot[] slots, Transform batchParent, Canvas canvas, bool reCreateBatches)
        {
            if (slots == null || batchParent == null) return;

            //setup the gameobjects that will hold each sub-GameObject of our slots.
            //In order for batching to work well we should have one GameObject batch
            //for each child of the slots. We'll sample the first slot of the grid (if any)
            //to see how many batches we'll need.
            if (reCreateBatches)
            {
                if (batches != null && batches.Length > 0)
                {
                    foreach (var go in batches) GameObject.Destroy(go);
                }
                if (slots != null && slots.Length > 0 && slots[0].transform.childCount > 0)
                {
                    batches = new GameObject[slots[0].transform.childCount];
                    for (int i = 0; i < batches.Length; i++)
                    {
                        //we can help UGui's batching by putting non-overlapping
                        //elements together in a single GameObject. Each GO will
                        //represent a batch.
                        batches[i] = new GameObject(slots[0].transform.GetChild(i).name+" Batch");
                        batches[i].AddComponent<RectTransform>();
                        batches[i].transform.SetParent(batchParent, false);

                    }
                }
            }

            //move all of the grid slots' children to the batching objects.
            SlotBatch child = null;
            List<SlotBatch> temp = new List<SlotBatch>(5);
            foreach (PGISlot slot in slots)
            {
                //collect a list of all batchable GameObjects under this slot.
                temp.Clear();
                for(int i = 0; i < slot.transform.childCount; i++)
                {
                    child = slot.transform.GetChild(i).GetComponent<SlotBatch>();
                    if (child != null) temp.Add(child);
                }

                //move the batchable objects
                for(int i = 0; i < temp.Count; i++)
                {
                    temp[i].transform.SetParent(batches[i].transform, true);
                }
                
            }
        }

        /// <summary>
        /// Handles the BeginDrag event trigger from a slot. This method
        /// will cache the item being manipulated before removing it from
        /// its storage location.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        void OnDragBegin(PointerEventData eventData)
        {
            if (DisableDragging) return;
            //Usually happens when we drag an empty slot
            if (DraggedItem != null) return;

            //get the contents of the slot we started dragging,
            //cache it, and then remove it from the grid slot.
            PGISlot slot = eventData.pointerDrag.GetComponent<PGISlot>();
            DraggedItem = new CachedItem(slot.Item, slot, slot.Model, slot.View);
            if (slot.IsEquipmentSlot)
            {
                if (slot.Model.Unequip(DraggedItem.Item) != null)
                {
                    //send unequip events to the item and slot when we begin dragging
                    DraggedItem.Item.TriggerUnequipEvents(slot.Model, slot);
                }
                else
                {
                    throw new UnityException("There was an error while attempting to unequip item.");
                }
            }
            else
            {
                if (slot.Model.Remove(DraggedItem.Item) == null) throw new UnityException("There was an error while attempting to remove the item from the inventory grid.");
            }

            //display the icon that follows the mouse cursor
            SetDragIcon(DragIcon, DraggedItem, slot);
            OnSlotDragBegin.Invoke(eventData, slot);
        }

        /// <summary>
        /// Handles the EndDrag event trigger from the slot or location that began the drag.
        /// This method resets the dragging state and removes the previously
        /// cached results. It also returns the item to its original location
        /// or removes the item from the inventory depending on any previously
        /// fired Drop triggers.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        void OnDragEnd(PointerEventData eventData)
        {
            //make sure this view's model's cache is reset
            Model.ResetSwapCache();

            //Usually happens when we drag an empty slot
            if (DraggedItem == null)
            {
                OnSlotDragEnd.Invoke(eventData, null);
                return;
            }
            OnSlotDragEnd.Invoke(eventData, DraggedItem.Slot);

            //make sure the item's model's cache is reset
            DraggedItem.Model.ResetSwapCache();


            GameObject enteredGO = eventData.pointerEnter;
            PGISlot dropSlot = null;
            if (enteredGO == null)
            {
                if (DraggedItem.View.DisableWorldDropping)
                {
                    ReturnDraggedItemToSlot();
                    DeselectAllViews();
                    DraggedItem = null;
                    DragIcon.SetIconActive(DragIcon.ActiveIcon.None);
                    ResetDragIcon(DragIcon);
                    return;
                }
                else
                {
                    //This is where we drop items from inventories entirely.
                    //The location that was chosen to end the drag was
                    //completely empty (including UI elements). So we
                    //will remove item from the inventory completely.
                    
                    //make sure we return this item to normal orientation when removing it
                    DraggedItem.Item.Rotate(PGISlotItem.RotateDirection.None);
                    
                    //trigger removal and unequip events
                    //NOTE: Unequip happens when drag starts now, so we don't need to trigger it here.
                    //if(DraggedItem.WasEquipped) DraggedItem.Item.OnUnequipped(DraggedItem.Model, DraggedItem.Slot);
                    DraggedItem.Item.TriggerRemoveEvents(DraggedItem.Model);

                    DraggedItem = null;
                    DragIcon.gameObject.SetActive(false);

                    DeselectAllViews();
                    ResetDragIcon(DragIcon);
                    return;
                }
            }
            else dropSlot = enteredGO.GetComponent<PGISlot>();


            if (dropSlot == null || dropSlot.View == null || dropSlot.View.DisableDropping)
            {
                ReturnDraggedItemToSlot();
            }
            else
            {
                //make sure we have a valid grid size for our item
                if (!dropSlot.IsEquipmentSlot &&
                    (DraggedItem.Item.CellHeight > dropSlot.Model.GridCellsY || DraggedItem.Item.CellWidth > dropSlot.Model.GridCellsX))
                {
                    ReturnDraggedItemToSlot();
                }
                else
                {
                    //Here is where we perform our drop action
                    dropSlot = dropSlot.View.GetOffsetSlot(DraggedItem.Item, dropSlot);
                    //If the offset is the same AND the view is the same,
                    //then we simply return the item from whence it came.
                    if (dropSlot != DraggedItem.Slot)
                    {
                        //we need to manually trigger a drop and relocate the item.
                        //The question is: do we store it, or do we equip it?
                        if (!dropSlot.View.AssignItemToSlot(DraggedItem.Item,
                                                          dropSlot,
                                                          DraggedItem.WasEquipped,
                                                          DraggedItem.WasStored,
                                                          DraggedItem.Slot))
                        {
                            ReturnDraggedItemToSlot();
                        }
                    }
                    else
                    {
                        ReturnDraggedItemToSlot();
                    }
                }

            }

            DeselectAllViews();
            DraggedItem = null;
            ResetDragIcon(DragIcon);
        }

        /// <summary>
        /// Handles the updating drag event. Provides highlighting and cell
        /// offsetting (to ensure the item is placed in a reasonable way on the grid).
        /// </summary>
        /// <param name="eventData">Event data.</param>
        void OnDrag(PointerEventData eventData)
        {
            //this can happen if we attempt to drag and empty slot
            if (DraggedItem == null) return;
            AppendClearList(this);


            //figure out highlighting and cell offsets.
            PGISlot dropSlot = null;
            if (eventData.pointerEnter != null)
            {
                dropSlot = eventData.pointerEnter.GetComponent<PGISlot>();

                //clear all grids involved
                //kinda slow but I'm lazy right now
                DeselectAllViews();

                if (dropSlot != null)
                {
                    //Make sure the view is added to the dirty highlighting list, the highlight the dragged item
                    AppendClearList(dropSlot.View);
                    if (dropSlot.View != null) dropSlot.View.SelectSlot(dropSlot, DraggedItem.Item);
                }
            }


            //make the dummy icon follow the mouse
            DragIcon.transform.position = PGICanvasMouseFollower.GetPointerPosOnCanvas(UICanvas, PGIPointer.GetPosition());

        }

        /// <summary>
        /// Helper method used to initialize the DragIcon when it becomes visible.
        /// </summary>
        void SetDragIcon(DragIcon icon, CachedItem draggedItem, PGISlot slot)
        {
            switch (draggedItem.Item.RotatedDir)
            {
                case PGISlotItem.RotateDirection.None:
                    {
                        icon.Icon.transform.eulerAngles = Vector3.zero;
                        icon.Icon3D.transform.eulerAngles = Vector3.zero;
                        break;
                    }
                case PGISlotItem.RotateDirection.CW:
                    {
                        icon.Icon.transform.eulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
                        icon.Icon3D.transform.eulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
                        break;
                    }
                case PGISlotItem.RotateDirection.CCW:
                    {
                        icon.Icon.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
                        icon.Icon3D.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
                        break;
                    }
            }

            if (float.IsInfinity(CellScaleX) || float.IsInfinity(CellScaleY) ||
               CellScaleX <= 0.01f || CellScaleY <= 0.01f)
            {
                //the slot sizes are probably too small to see.
                //This is mostly likely due to using an
                //inventory with a grid size of 0,0.
                //Use the slot size in this case.
                icon.GetComponent<RectTransform>().sizeDelta = slot.GetComponent<RectTransform>().sizeDelta * 0.9f;
            }
            else
            {
                //use a size roughly corresponding to the grid display's size
                //NOTE: we need to reverse them if the item has been rotated
                //Apparently the sizeDelta is absolute and does not take rotation into account.
                if(draggedItem.Item.Rotated)
                    icon.GetComponent<RectTransform>().sizeDelta = new Vector2((CellScaleX * 0.9f) * draggedItem.Height, (CellScaleX * 0.9f) * draggedItem.Width);
                else icon.GetComponent<RectTransform>().sizeDelta = new Vector2((CellScaleX * 0.9f) * draggedItem.Width, (CellScaleX * 0.9f) * draggedItem.Height);
            }

            //Display our icon image, either a sprite or a mesh.
            //icon.gameObject.SetActive(true); //activate first or GetComponent will fail
            if (draggedItem.Item.IconType == PGISlotItem.IconAssetType.Sprite)
            {
                icon.SetIconActive(DragIcon.ActiveIcon.Icon2D);
                icon.Icon.sprite = draggedItem.Item.Icon;
                icon.Icon3D.material = null;
                icon.Icon3D.Mesh = null;
                
            }
            else
            {
                icon.SetIconActive(DragIcon.ActiveIcon.Icon3D);
                icon.Icon.sprite = null;
                icon.Icon3D.material = draggedItem.Item.IconMaterial;
                icon.Icon3D.Rotation = draggedItem.Item.IconOrientation;
                icon.Icon3D.Mesh = draggedItem.Item.Icon3D;
                
            }

           
        }

        /// <summary>
        /// Helper method used to reset the drag icon. This should be
        /// called anytime a drag ends for any reason otherwise 3D mesh
        /// icons might not update correctly next time due to cached values
        /// within the CanvasMesh.
        /// </summary>
        void ResetDragIcon(DragIcon icon)
        {
            //We have to do this otherwise it won't update properly next time
            /*
            var mesh = icon.GetComponent<Image3D>();
            mesh.Mesh = null;
            mesh.material = null;
            mesh.enabled = false;
            */
            icon.Icon.transform.eulerAngles = Vector3.zero;
            icon.Icon3D.transform.eulerAngles = Vector3.zero;
            icon.SetIconActive(DragIcon.ActiveIcon.None);
            icon.gameObject.SetActive(false);
        }

        /// <summary>
        /// Helper method for assigning an item an equipment slot.
        /// </summary>
        /// <returns><c>true</c>, if dragged item to slot was assigned, <c>false</c> otherwise.</returns>
        /// <param name="item">The item being assigned.</param>
        /// <param name="dest">The destination slot to assign the item to.</param>
        /// <param name="wasEquipped">Set to <c>true</c> if the item being assigned was previously in an equipment slot.</param>
        /// <param name="wasStored">Set to <c>true</c> if the item being assigned was previously stored in an inventory. Equipped, or in a grid.</param>
        /// <param name="previousSlot">The equipment slot if any that the item was previously equipped to. This can be null unless <c>wasEquipped</c> is <c>true</c></param>
        /// <param name="equipIndex">Equip index.</param>
        bool AssignItemToSlot(PGISlotItem item, PGISlot dest, bool wasEquipped, bool wasStored, PGISlot previousSlot)
        {
            if (dest == null) return false;

            if (dest.IsEquipmentSlot)
            {

                PGISlotItem swappedItem;
                swappedItem = dest.Model.SwapEquip(item, previousSlot, dest);
                if (swappedItem != null)
                {
                    if (swappedItem == item)
                    {
                        //we didn't swap
                        if (previousSlot.Model != this.Model)
                        {
                            item.TriggerRemoveEvents(previousSlot.Model);
                            item.TriggerStoreEvents(dest.Model);
                        }
                        //if(wasEquipped) item.TriggerUnequipEvents(previousSlot.Model, previousSlot);
                        item.TriggerEquipEvents(Model, dest);//TODO: Confirm we don't need to test for equipment here
                        return true;

                    }
                    else
                    {
                        //since we didn't drag the swapped item, we'll need to trigger
                        //its unequip events now. We'll do this before we trigger the
                        //other stuff just so the order of events stays somewhat normal
                        //(as if we dragged the item normally)
                        //As for the other item... it was being dragged so it should
                        //already have had its unequip events triggerd
                        if (wasEquipped) swappedItem.TriggerUnequipEvents(dest.Model, dest);


                        //It's possible we moved this item to another container.
                        //If we did, we'll want to trigger some store/remove triggers.
                        if (item.Model != previousSlot.Model)
                        {
                            item.TriggerRemoveEvents(previousSlot.Model);
                            if (item.Model != null) item.TriggerStoreEvents(dest.Model);
                        }
                        if (swappedItem.Model != dest.Model)
                        {
                            swappedItem.TriggerRemoveEvents(dest.Model);
                            if (swappedItem.Model != null) swappedItem.TriggerStoreEvents(previousSlot.Model);
                            return true;
                        }

                        //this stuff happens when swapping between grid and equipment slot
                        if (swappedItem.IsEquipped)
                            swappedItem.TriggerEquipEvents(swappedItem.Model, previousSlot);
                        else if (!wasEquipped) swappedItem.TriggerUnequipEvents(swappedItem.Model, dest);
                        item.TriggerEquipEvents(item.Model, dest);

                        return true;
                    }

                }
            }
            else
            {
                //we are moving this to a normal grid slot
                if (dest.Model.Store(item, dest.xPos, dest.yPos))
                {
                    //if it wasn't stored before (which by all means it should have
                    //been if we are dragging it) then it will be now.
                    if (previousSlot.Model != this.Model)
                    {
                        item.TriggerRemoveEvents(previousSlot.Model);
                        item.TriggerStoreEvents(dest.Model);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Helper method used to return a previously cached and dragged item to the
        /// location it came form when the drag started.
        /// </summary>
        /// <returns><c>true</c>, if dragged item was returned, <c>false</c> otherwise.</returns>
        bool ReturnDraggedItemToSlot()
        {
            if (DraggedItem == null) return false;

            //End the drag like normal. Return it to it's origial position
            //either in the grid on in an equipment slot.

            //NOTE: I am passing false to the 'checkCanMethods' flags here to avoid triggering them
            //on any attached scripts. This can be a little dangerous seeing as it is possible
            //something has changed in the model during the drag - Or users might find
            //this behaviour somewhat confusing. However, this fixes more problems than it solves right now.
            //Namely, the 'LinkedEquipSlots' example script breaks when trying to return an item
            //because its 'CanEquip' method will fail when dragging and dropping into the same slot.
            if (DraggedItem.EquipIndex >= 0)
            {
                DraggedItem.Model.Equip(DraggedItem.Item, DraggedItem.EquipIndex, false);
                DraggedItem.Item.TriggerEquipEvents(DraggedItem.Model, DraggedItem.Slot);
            }
            else
            {
                DraggedItem.Model.Store(DraggedItem.Item, DraggedItem.xPos, DraggedItem.yPos, false);
            }

            return true;
        }

        /// <summary>
        /// Updates the entire grid UI to match the state of the model.
        /// </summary>
        void UpdateDirtyGrid()
        {
            //Reset all slots to default size.
            ResetSlotRange(0, 0, Model.GridCellsX, Model.GridCellsY);
            foreach(PGISlot slot in Slots)
            {
                
                if (slot != null)
                {
                    if (Model.IsInitialized)
                    {
                        PGISlotItem item = Model.GetSlotContents(slot.xPos, slot.yPos);
                        if (item != null)
                        {
                            //There is an item in this slot. Make sure the slot is the right size.
                            //Order is important here. It ensures that 3D icon meshes will scale correctly
                            //in the event that they don't constantly check (mesh update interval is negative)
                            ResizeSlot(item.xInvPos, item.yInvPos, item.CellWidth, item.CellHeight);
                            slot.AssignItem(item);
                        }
                        else
                        {
                            //This will ensure that 3D icon meshes will scale correctly
                            //in the event that they don't constantly check (mesh update interval is negative)
                            slot.AssignItem(null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the grid cell position given the
        /// number of cells and the size of the parenting object.
        /// </summary>
        /// <returns>The cell position.</returns>
        /// <param name="cellX">X position on grid.</param>
        /// <param name="cellY">Y Position on the grid.</param>
        /// <param name="slotWidth">Slot width.</param>
        /// <param name="slotHeight">Slot height.</param>
        Vector2 CalculateCellPos(int cellX, int cellY, int slotWidth = 1, int slotHeight = 1)
        {
            float yDir = (VerticalOrder == VerticalOrdering.TopToBottom) ? -1.0f : 1.0f;
            float xDir = (HorizontalOrder == HorizontalOrdering.LeftToRight) ? 1.0f : -1.0f;
            float cellPosX = (float)(cellX * CellScaleX) * xDir;
            float cellPosY = (float)(cellY * CellScaleY) * yDir;
            float cellHalfWidth = ((CellScaleX * slotWidth) * 0.5f) * xDir;
            float cellHalfHeight = ((CellScaleY * slotHeight) * 0.5f) * yDir;

            float parentOffsetX = (ParentRect.rect.width * 0.5f) * xDir;
            float parentOffsetY = (ParentRect.rect.height * 0.5f) * yDir;

            return new Vector2(cellPosX + cellHalfWidth - parentOffsetX,
                                   cellPosY + cellHalfHeight - parentOffsetY);

        }

        /// <summary>
        /// Returns the <see cref="PGISlot"/> found in the given grid coordinates. This represents
        /// a <see cref="PGIView"/> grid, not the internal grid of the model.
        /// </summary>
        /// <returns>The slot cell of this view.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        PGISlot GetSlotCell(int x, int y)
        {
            if (x < Model.GridCellsX && y < Model.GridCellsY)
                return Slots[(y * Model.GridCellsX) + x];

            return null;
        }

        /// <summary>
        /// Resizes the slot at the given location to the given grid-cell size.
        /// </summary>
        /// <returns><c>true</c>, if slot was resized, <c>false</c> otherwise.</returns>
        /// <param name="slot">Slot.</param>
        /// <param name="slotWidth">Slot width.</param>
        /// <param name="slotHeight">Slot height.</param>
        bool ResizeSlot(int x, int y, int width, int height)
        {
            //check for items that aren't actually in a slot (this is a defense against resizing a slot for an item that
            //was recently removed from a grid slot but the model isn't in sync just yet).
            if (x < 0 || y < 0) return false;

            float i = ParentRect.rect.width / Model.GridCellsX;
            float j = ParentRect.rect.height / Model.GridCellsY;
            PGISlot initial = this.GetSlotCell(x, y);

            //now, disable any slots that we will be stretching this slot overtop of
            for (int t = y; t < y + height; t++)
            {
                for (int s = x; s < x + width; s++)
                {
                    PGISlot slot = this.GetSlotCell(s, t);
                    slot.GridWidth = width;
                    slot.GridHeight = height;
                    if (s == x && t == y)
                    {
                        //this is the cell we are resizing. Set the new size
                        float w = i * width;
                        float h = j * height;
                        RectTransform rect = slot.GetComponent<RectTransform>();
                        rect.sizeDelta = new Vector2(w, h);
                        rect.anchoredPosition = CalculateCellPos(s, t, width, height);
                        if (BatchSlots) slot.UpdateSlotSize();
                    }
                    else
                    {
                        //this is a cell that we are disabling because
                        //the resized cell will be covering it
                        slot.gameObject.SetActive(false);
                        slot.OverridingSlot = initial;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Helper method used to reset a range of cell-slots to 
        /// a normal condition. Used mostly to restore slots that
        /// were previously disabled and covered up when another
        /// slot had to grow in size.
        /// </summary>
        /// <param name="xPos">X position.</param>
        /// <param name="yPos">Y position.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="active">Optional active flag to pass to the <see cref="PGISlot.SetActive"/> method.</param>
        void ResetSlotRange(int xPos, int yPos, int width, int height, bool active = true)
        {
            if (ParentRect == null || Model == null) return;

            float i = ParentRect.rect.width / Model.GridCellsX;
            float j = ParentRect.rect.height / Model.GridCellsY;

            for (int y = yPos; y < yPos + height; y++)
            {
                for (int x = xPos; x < xPos + width; x++)
                {
                    PGISlot slot = this.GetSlotCell(x, y);
                    if (slot != null)
                    {
                        slot.GridWidth = 1;
                        slot.GridHeight = 1;
                        RectTransform rect = slot.GetComponent<RectTransform>();
                        rect.sizeDelta = new Vector2(i, j);
                        rect.anchoredPosition = CalculateCellPos(x, y, 1, 1);
                        slot.gameObject.SetActive(active);
                        slot.OverridingSlot = null;
                        if (BatchSlots) slot.UpdateSlotSize();
                    }
                }
            }
        }

        /// <summary>
        /// Using the currently hovered slot and inventory size of the selected item,
        /// this method determines the final offset location to place the item so
        /// that it will fit the grid in the nearest selected set of grid slots
        /// and fits within the grid itself.
        /// <remarks>
        /// This aids in a refinement nicety for the user as the placement of items
        /// will seem more natural when the item snaps to the closest set of grid
        /// cells when they are placing larger inventory items.
        /// </remarks>
        /// </summary>
        /// <returns>The slot to actually highlight or store an item in based on item-size offsets.</returns>
        /// <param name="item">The item whose size will be used for offset calculations.</param>
        /// <param name="slot">The original slot being targedt for a drop or hilight.</param>
        PGISlot GetOffsetSlot(PGISlotItem item, PGISlot slot)
        {
            if (slot.IsEquipmentSlot) return slot;
            int offsetX = slot.xPos;
            int offsetY = slot.yPos;
            Vector2 quad = slot.GetLocalMouseCoords();
            if (item == null) return slot;
            if (item.CellWidth > 1)
            {
                if (HorizontalOrder == HorizontalOrdering.LeftToRight)
                {
                    //offset based on the quadrant of the selected cell
                    if ((item.CellWidth & 0x1) == 1) offsetX -= ((int)(item.CellWidth / 2)); //odd width
                    else if (quad.x < 0.0f) offsetX -= ((int)(item.CellWidth / 2)); //even width
                    else offsetX -= ((int)(item.CellWidth / 2) - 1);//even width
                }
                else
                {
                    //offset based on the quadrant of the selected cell
                    if ((item.CellWidth & 0x1) == 1) offsetX -= ((int)(item.CellWidth / 2)); //odd width
                    else if (quad.x > 0.0f) offsetX -= ((int)(item.CellWidth / 2)); //even width
                    else offsetX -= ((int)(item.CellWidth / 2) - 1);//even width
                }
            }

            if (item.CellHeight > 1)
            {
                if (VerticalOrder == VerticalOrdering.TopToBottom)
                {
                    //offset based on the quadrant of the selected cell
                    if ((item.CellHeight & 0x1) == 1) offsetY -= ((int)(item.CellHeight / 2)); //odd height
                    else if (quad.y > 0.0f) offsetY -= ((int)(item.CellHeight / 2)); //even height
                    else offsetY -= ((int)(item.CellHeight / 2) - 1);//event height
                }
                else
                {
                    //offset based on the quadrant of the selected cell
                    if ((item.CellHeight & 0x1) == 1) offsetY -= ((int)(item.CellHeight / 2)); //odd height
                    else if (quad.y < 0.0f) offsetY -= ((int)(item.CellHeight / 2)); //even height
                    else offsetY -= ((int)(item.CellHeight / 2) - 1);//even height
                }
            }
            //keep the final location within the grid
            if (offsetX < 0) offsetX = 0;
            if (offsetX > slot.Model.GridCellsX || offsetX + item.CellWidth > slot.Model.GridCellsX) offsetX = slot.Model.GridCellsX - item.CellWidth;
            if (offsetY < 0) offsetY = 0;
            if (offsetY > slot.Model.GridCellsY || offsetY + item.CellHeight > slot.Model.GridCellsY) offsetY = slot.Model.GridCellsY - item.CellHeight;

            return slot.View.GetSlotCell(offsetX, offsetY);
        }

        /// <summary>
        /// Handles highlighting effects when hovering over a grid slot
        /// while performing a drag. This method calculates
        /// the nearest central location for placing an item within
        /// the grid and highlights all cells that will be
        /// used for storage.
        /// </summary>
        /// <param name="slot">The slot that the pointer is currently over.</param>
        /// <param name="item">The item being dragged or dropped.</param>
        void SelectSlot(PGISlot slot, PGISlotItem item)
        {
            //if the item is too big, just highlight everything in the grid as invalid and be done with it.
            if (!slot.IsEquipmentSlot && (item.CellHeight > this.Model.GridCellsY || item.CellWidth > this.Model.GridCellsX))
            {
                foreach (var s in slot.View.Slots) s.HighlightColor = InvalidColor;
                return;
            }

            if (slot.IsEquipmentSlot)
            {
                //check highlighting for equipment slots here
                if (slot.Blocked) slot.HighlightColor = BlockedColor;
                else if (slot.Model.CanSwap(item, slot))
                {
                    slot.HighlightColor = HighlightColor;
                }
                else if (slot.Model.CanSocket(item, slot.Item))
                {
                    slot.HighlightColor = SocketValidColor;
                }
                else
                {
                    slot.HighlightColor = InvalidColor;
                }

                return;
            }

            //check grid slots for special-case highlighting like socketables and stackables
            Color color;
            if (item == null || slot == null) return;
            var offset = slot.View.GetOffsetSlot(item, slot);
            if (slot.Model.CanStack(item, offset.xPos, offset.yPos) || slot.Model.CanStore(item, offset.xPos, offset.yPos))
                color = HighlightColor;
            else if (slot.Model.CanSocket(item, slot.Item))
                color = SocketValidColor;
            else color = InvalidColor;


            //find out which slots to highlight based on current hover location
            //and the neighboring slots
            for (int y = offset.yPos; y < offset.yPos + item.CellHeight; y++)
            {
                for (int x = offset.xPos; x < offset.xPos + item.CellWidth; x++)
                {
                    PGISlot start = slot.View.GetSlotCell(x, y);
                    start.HighlightColor = color;
                    if (start.OverridingSlot != null) start.OverridingSlot.HighlightColor = color;
                }
            }
        }

        /// <summary>
        /// Helper method used to append grid views to a list
        /// that can later be cleared. Used for processing what
        /// view's slots need to be de-highlighted.
        /// </summary>
        /// <param name="view">The view to add to the list.</param>
        static void AppendClearList(PGIView view)
        {
            if (!ClearList.Contains(view)) ClearList.Add(view);
        }

        /// <summary>
        /// Removes all drag-related highlighting from all grid cells
        /// and equipment slots in this <see cref="PGIView"/>.
        /// </summary>
        public void DeselectAllSlots()
        {
            foreach (PGISlot slot in Slots)
            {
                slot.RestoreHighlight(NormalColor);
            }
            if (Model.Equipment != null)
            {
                foreach (PGISlot slot in Model.Equipment)
                {
                    if (slot.Blocked) slot.HighlightColor = BlockedColor;
                    else slot.RestoreHighlight(NormalColor);
                }
            }
        }

        /// <summary>
        /// Helper method used to remove selection from all previously stored grid views.
        /// This can be kinda slow since it will inevitably cycle through all slots
        /// in all inventories that were dragged over during a drag operation.
        /// <seealso cref="PGIView.AppendClearList"/>
        /// <seealso cref="PGIView.DeselectAllSlots"/>
        /// 
        /// <remarks>
        /// TODO: This could use a good amount of optimizing. Likely, the
        /// OnDeselectAll method could use a dirty list to only clear
        /// slots that have changed recently.
        /// </remarks>
        /// </summary>
        static void DeselectAllViews()
        {
            foreach (PGIView view in ClearList)
            {
                if (view != null) view.DeselectAllSlots();
            }

        }

        /// <summary>
        /// Handles the previously registered <see cref="PGISlot.OnHover"/> event 
        /// when the pointer first enters a <see cref="PGISlot"/> and invokes
        /// this view's <see cref="PGIView.OnHoverSlot"/> event.
        /// </summary>
        /// <param name="eventData">The pointer event data that triggered the event.</param>
        /// <param name="slot">The slot that the pointer entered.</param>
        void OnHoverEvent(PointerEventData eventData, PGISlot slot)
        {
            if (DraggedItem == null && slot.Item != null)
            {
                OnHoverSlot.Invoke(eventData, slot);
            }
        }

        /// <summary>
        /// Handles the previously registered <see cref="PGISlot.OnEndHover"/> event 
        /// when the pointer leaves a <see cref="PGISlot"/> and invokes
        /// this view's <see cref="PGIView.OnEndHoverSlot"/> event.
        /// </summary>
        /// <param name="eventData">The pointer event data that triggered the event.</param>
        /// <param name="slot">The slot that the pointer exited.</param>
        void OnEndHoverEvent(PointerEventData eventData, PGISlot slot)
        {
            OnEndHoverSlot.Invoke(eventData, slot);
        }

        /// <summary>
        /// Handles the previously registered <see cref="PGISlot.OnClick"/> event 
        /// when the pointer clicks on a <see cref="PGISlot"/> and invokes
        /// this view's <see cref="PGIView.OnClickSlot"/> event.
        /// </summary>
        /// <param name="eventData">The pointer event data that triggered the event.</param>
        /// <param name="slot">The slot that was clicked.</param>
        void OnClickSlotHandler(PointerEventData eventData, PGISlot slot)
        {
            OnClickSlot.Invoke(eventData, slot);
        }
        #endregion

    }
}