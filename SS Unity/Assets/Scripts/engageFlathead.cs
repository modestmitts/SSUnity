using UnityEngine;
using System.Collections;

public class engageFlathead : MonoBehaviour
{
    // Player Position Adjustment
    //Position(0.36f, 0.301f, 0.762f)
    //Rotation(0, 180,  340)
    //Scale (0.5, 0.5, 0.5)

    // Bolt Position Adjustment
    //Position(-0.012, -0.07499, 0.0056)
    //Rotation(0, 90,  0)
    //Scale (0.0333, 0.0333, 0.0333)

    public float turnSpeed = 0.15f;

    // These positi
    private Vector3 flatheadPosition = new Vector3(0.0f, 0.0f, 8.23f);
    private Vector3 flatheadScale = new Vector3(15.0f, 15.0f, 15.0f);

    private Vector3 PlayerAdjust = new Vector3(0.477f, 0.446f, 0.569f);
    private Vector3 PlayerScale = new Vector3(1.0f, 1.0f, 1.0f);

    private bool adjusted = false;
    private bool ready = false;
    private BoltSpecs boltstats;
    private int currentPos;
    private Rigidbody RB;
    private PlayerRaycast PlRay;
    /****
     * flatheadEquipped
     * This is called on the Event of the flathead being Equipped via the Power Grid Inventory
     * This positions the flathead in relationship to the Player (parent), and calls PlayerRaycast to set
     * inHand to flathead.
     */

    public void toolEquipped()
    {
        // 

        RB = this.GetComponent<Rigidbody>();

        PlRay = transform.parent.GetComponentInChildren<PlayerRaycast>();


        //boltstats = transform.parent.GetComponent(typeof(BoltSpecs)) as BoltSpecs;

        Vector3 playerPos;
        PlRay.SetinHand(1, this.gameObject);

        //playerPos = GameObject.Find("Player").transform.position;
        playerPos = transform.parent.transform.position;

        RB.isKinematic = true;
        this.transform.position = playerPos;

        this.transform.localPosition = Vector3.zero;
        this.transform.localPosition = PlayerAdjust;
        //flathead.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        this.transform.localRotation = Quaternion.Euler(-68.59f, 0f, -22.71f);
        this.transform.localScale = PlayerScale;
    }

    /*  flatheadBolt
     * This Function resides in the flathead Tool. 
     * When the Toolraycast strikes a Collider with a "hex" tag. This function is called, and the 
     * flathead Tool becomes a child of the bolt, and attaches itself to it. (Adjusted)
     * While the function is "adjusted" if the player presses a mouse button, it will twist the bolt
     * and flathead child it is supposed to. 
     * If the player moves the ToolRaycast off the "Bolt" collider, the flathead snaps back to the person parent.
   
    ****************************************************************/
 public void engageBolt(GameObject newParent, bool work)
    {
        if (work)
        {
            if (!adjusted)
            {
                // Make the bolt the parent of the tool
                transform.parent = newParent.transform;
                // Move to the zero position relative to the bolt
                this.transform.localPosition = Vector3.zero;

                // Get the boltstats of the bolt we are attaching to
                boltstats = transform.parent.GetComponent(typeof(BoltSpecs)) as BoltSpecs;

                this.transform.localRotation = Quaternion.Euler(0f, 180f, 90f);
                // Move the tool in position relative to the bolt
                this.transform.localPosition = flatheadPosition;
                // Scale up
                this.transform.localScale = flatheadScale;

                //Just in case, zero out the rotation of the tool object (Not the parent)

                //flathead.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                //Not sure what's going on here???
                //flathead.transform.parent = this.transform;
                //flathead.transform.position = this.transform.position;                
                ready = true;
                adjusted = true;
            }

        }
        else
        {
            if (adjusted)
            {
                ready = false;
                boltstats = null;
                transform.parent = newParent.transform;
                this.transform.localPosition = PlayerAdjust;
                //flathead.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                this.transform.localRotation = Quaternion.Euler(-70.9f, -59.5f, 59.6f);
                //flathead.transform.parent = this.transform;
                //flathead.transform.position = this.transform.position; 
                this.transform.localScale = PlayerScale;
                adjusted = false;
            }
        }

    }

    void Update()
    {
        if (ready)
        {
            bool tighten = true;
            // If Right Button Down
            if (Input.GetMouseButton(1))
            {
                boltstats.TurnBolt(tighten, turnSpeed);
            }
            if (Input.GetMouseButton(0))
            {
                boltstats.TurnBolt(!tighten, -turnSpeed);
            }

        }
    }
}
