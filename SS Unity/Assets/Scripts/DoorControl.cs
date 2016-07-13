using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DoorControl : MonoBehaviour 
{

    public Animator anim; // Grab the Controller
    public string doorName; 
    public bool isOpen;
    public bool isLocked;

    public Toggle OpenClose;
    public Toggle LockUnlock;

	// All doors start closed and unlocked, unless I make it otherwise
	void Start () 
    {
        anim = GetComponent<Animator>();
        // If the door is not locked, and is set Open, open the door.
        if ((!isLocked) && (isOpen)) anim.SetBool("isOpen", isOpen);

        // If the door is locked and the door is set Open, set it back to Close 
        // Can't open a locked door
        if ((isLocked) && (isOpen)) isOpen = false;
	}
	
	// The Player raycast hit will call this function if the door mesh is clicked on
	public void ToggleDoor()
    {
        //If Door is open, set to close
        if (isOpen)  isOpen = false;

        //If Door is closed, and unlocked, set to open
        else if (isLocked == false) isOpen = true;
       
        anim.SetBool("isOpen", isOpen);

        if (OpenClose) OpenClose.isOn = isOpen; 
   	}

    //This will close a door that is opened, and lock it, or just lock a closed door
    public void ToggleLock()
    {
        isLocked = !isLocked;

        //If Locked and Open, close the door
        if ((isLocked) && (isOpen)) ToggleDoor();
          
        if (LockUnlock) LockUnlock.isOn = isLocked;
    }
}
