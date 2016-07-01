using UnityEngine;
using System.Collections;

/******************************
 * HatchControl
 * The way this SHOULD work is that the ToolRaycast will hit a hatch with an
 * attached collider with a "panel" tag. If the player also hits the left
 * mouse button, it will call this function that resides in the hatch.
 * 
 * Depending on the hatches "Open" bool, it will run the appropriate open
 * or close animation. 
 * 
 * 
 * ****************************/

public class HatchControl : MonoBehaviour 
{
    public Animation open_anim;
    public Animation close_anim;

    public bool open;
    public bool ready;

    void Start()
    {
        ready = false;
    }

    public void readyHatch()
    {
        ready = true;
    }

    public void unreadyHatch()
    {
        ready = false;
    }

    void Update()
    {
        if ((ready) && (Input.GetMouseButton(0))) toggleHatch();
    }

	public void toggleHatch () 
    {
        print("In Toggle Hatch!");
        if ((open) && (close_anim != null))
            close_anim.Play();
        else if (open_anim != null) open_anim.Play();

        open = !open;              
        
	}
}
