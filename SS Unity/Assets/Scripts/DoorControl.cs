using UnityEngine;
using System.Collections;

public class DoorControl : MonoBehaviour 
{

    public Animator anim; // Grab the Controller
    public string doorName; 
    public bool isOpen;
    public bool isLocked;
    public bool isDouble;
    public GameObject OtherDoor;

	// All doors start closed and unlocked, unless I make it otherwise
	void Start () 
    {
        anim = GetComponent<Animator>();
        isOpen = false;
        isLocked = false;	
	}
	
	// The Player raycast hit will call this function if the door mesh is clicked on
	public void ToggleDoor(int num)
    {
        if (isOpen)  isOpen = false;
        else if (isLocked == false) isOpen = true;
       
        anim.SetBool("isOpen", isOpen);
        if ((isDouble) && (num == 0)) OtherDoor.GetComponent<DoorControl>().ToggleDoor(1);
   	}

    //This will allow an open door to lock (that is it will lock the door closed, when it is closed
    public void ToggleLock(int num)
    {
        isLocked = !isLocked;
        if ((isDouble) && (num == 0)) OtherDoor.GetComponent<DoorControl>().ToggleLock(1);
    }
}
