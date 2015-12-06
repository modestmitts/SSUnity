Thank you for purchasing Power Grid Inventory.
Please read the Getting Started Guide, Manual, and Doxygen generated HTML documentation for more details on working with PGI.
If you have any questions, concerns, or suggestions, please feel free to contact me any time.

Contact: pgi-support@ancientcraftgames.com
Online Documentation: http://ancientcraftgames.com/pgi/docs/

Youtube Videos:
  Features:       https://www.youtube.com/watch?v=iPkzpzbvreM
  5 Minute Setup: https://www.youtube.com/watch?v=mtwW0lrCnIU
  
-----------------
 Future Features
-----------------
- Reference slots (so that you can use PGI for hotkeys and macros)
- Built-in Save/Load?
- Better support for Custom Views
- PGIpdndView for pointer drag n' drop interface
- PGIpcndView for pointer click 'n drop interface
- PGIgpView for gamepad support


-----------------
 Version History
-----------------
1.4.0: 11/11/2015
• Items can now be rotated using PGISlotItem.Rotate(PGISlotItem.RotationDirection). Rotation is in the form of three non-relative states. 'None', 'Clockwise', and 'Counter-Clockwise'. When rotating items in an inventory they may shift position slightly in order to attempt to fit into available spaces. *
• Added an auto-arrange method for models that can optionally support rotation of items. This is still very experimental and will likely give sub-optimal results in some cases. *
• Removed update methods from PGIView and PGISlot. This should boost performance, specially in the case of large grids.
• Greatly simplified the internal drity updating system in the model and how the view uses it. The model now triggers an event of the type 'public event DirtyEvent OnUpdateDirty' when something changes. This should greatly increase robustness, maintainablity, extensibility, and reduce bugs for future updates. It should also increase performance in the case of large inventory grids due to the fact that each slot no longer has its own update. The view might take a little extra time to update when the model changes since the whole thing is refreshed at once but this happens very infrequently so it should not be much of an issue.
• Fixed a bug that would occur when multiple 'Can..' event hooks where attached to the same event and the last one would override the 'CanPerformAction' flags set by all the previous ones. It now properly ensures that if the flag is set as false it stays that way and will not allow other events to reset it.
• Fixed an error in the slot batcher that would choke if any child elements of the slot did not have SlotBatch component attached.
• Added PGIView.UpdateViewStep() for forcing view to update.
• If a view's reference to a model is changed it now properly updates to reflect the state of the new model. **
• Model now properly supports having multiple active views attached to it at the same time. **

* These features are still very experimental and will often given results that are less than optimal. This will be an ongoing project to improve their performance and reliability.
** Currently equipment slots do not play well when switching models for a view and don't support multiple views at the same time period. The problem stems from the fact that they are attached directly to the model and bypass the view entirely. I may change this at some point in the future.


1.3.0: 9/3/2015
FEATURES:
• Addition of a 3D mesh UI element similar to Unity's 'UnityEngine.UI.Image'. Its fully qualified name is 'AncientCraftGames.UI.Image3D'.
• 3D meshes now supported for icons.
• Items can now have their own individual highlight colors that the view will use for a slot's highlight when that item is stored.
• Addition of an array of MonoBehaviour references to PGISlotItem. This way you can simply cast these references for commonly and frequently access components rather than use GetComponent<>() inside hooked events.
• Added a toggle to PGIView entitled 'DiableWorldDropping' than can be used to disable users from dropping items from the inventory by dragging them into an empty region.
• Added horizontal and vertical ordering settings to PGIView. These allow the view's grid to start at different corners when displayed. Note that internally the model's grid does not change, only the way the view interprets it.

UPDATES & FIXES:
• ***breaking change*** All classes are now under a namespace. The root namespaces are 'PowerGridInventory' and 'AncientCraftGames'.
• PGIView's 'Batch Equipment' and 'Batch Grid' have now been combined into a single 'Batch Slots' toggle.
• Slot elements that are unused or completely transparent are now disabled to help improve rendering performance.
• Batching works much better overall, even when PGIView.BatchSlots is disabled.
• CanSwap...() events are now only triggered a single time when the mouse first drags over an equipment slot rather than every time the mouse changes position whiled dragging.
• Mobile devices now no longer count it as a click when a user releases the mouse after hovering over a slot for more than 3/4 of a second. This should help normalize touch and mouse input more.
• Removal of the 'DragIcon' prefab for views. It is now implicitly created a runtime when the first PGIView becomes active and share between all views from that point on.
• Lots and lots of null-reference excpetions fixed.
• Spruced up the inspector views a bit to make them more readable and user-friendly.
• Changed a few minor public variable names. They all use [PreviouslySerializedAs] to ensure data isn't lost during update.
• When a view is disabled during a drag operation the drag is now canceled properly.
• When using 'Batch Slots' for PGIView, the z-depth for UI elements is now properly maintained.
• Fixed a bug in 'Socketed.EmptySockets' that would return the incorrect number.
• Added 'PGIModel.HasRoomForItem' method.


1.2.0: 8/6/2015
- FEATURE: Socketed and Socketable Items components.
- UPDATE: Added a system to re-arrange slots in order to greatly reduce draw calls at runtime (when in play mode). This should improve rendering performance in many situations.
- FIX: Removed an editor only import from PGIModel and added Conditional-compilation tag to PGIView that allow stand-alone builds to compile properly.
- FIX: Fixed null reference exception in PGIModel.
- FIX: Removed several leftover debug logs in PGIModel.


1.1.0: 6/25/2015
- FEATURE: PGIModel can now be set to automatically detect items that were added to, or removed from, its transform hierarchy.
- FEATURE: Model grid can now change rows and columns at runtime. Items will be dropped from inventory if this happens.
- FEATURE: Provided a 'PGIModel.Drop()' convenience method that automatically triggers the necessary events for unequipping and removing from inventory.
- CHANGE: Breaking Change: Moved many files from the 'Examples' folder to the 'Extensions' folder. Also changed some example component names.
- CHANGE: 'Inventory Item' container extension now uses double-clicks rather than right-clicks to open its PGIView.
- CHANGE: Added links to online documentation to Asset Store and readme file.
- CHANGE: Updated manuals to reflect new features. Expanded the user manual's explaination of the major components. Shortened the Getting Started guide.
- CHANGE: Changed e-mail support address that is given in documentation. Now points to pgi-support@ancientcraftgames.com
- FIX: Normalized input for mouse and touchscreen. They now work identically.
- FIX: Properly initialized the UnityEvent objects in PGIModel, PGIView, PGISlot, and PGISlotItem so they don't throw null exceptions when added to a GameObject at runtime.
- FIX: Fixed errors that pooped up when a model had no equipment slots but were being queried.
- FIX: Corrected drop-down menu titles for 'Linked Equip Slot' and 'Simple Pickup'.
- FIX: Inventory grid will now properly update its view in the editor when its parent's RectTransform is resized.
- FIX: Various spelling errors in Asset Store, User Manual, and Getting Started Guide.


1.0.0: 5/25/2015
- NEW: First release!

