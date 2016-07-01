using UnityEngine;
using System.Collections;

public class engagePhilips : MonoBehaviour
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
    private Vector3 philipsPosition = new Vector3(0.05f, 0.01f, 11.24f);
    private Vector3 philipsScale = new Vector3(6.0f, 6.0f, 6.0f);

    private Vector3 PlayerAdjust = new Vector3(0.65f, 0.03f, 0.86f);
    private Vector3 PlayerScale = new Vector3(0.5f, 0.5f, 0.5f);

    private bool adjusted = false;
    private bool ready = false;
    private BoltSpecs boltstats;
    private int currentPos;
    private Rigidbody RB;
    private PlayerRaycast PlRay;
    /****
     * wrenchEquipped
     * This is called on the Event of the Wrench being Equipped via the Power Grid Inventory
     * This positions the Wrench in relationship to the Player (parent), and calls PlayerRaycast to set
     * inHand to Wrench.
     */

    public void toolEquipped()
    {
        // 

        RB = this.GetComponent<Rigidbody>();

        PlRay = transform.parent.GetComponentInChildren<PlayerRaycast>();


        //boltstats = transform.parent.GetComponent(typeof(BoltSpecs)) as BoltSpecs;

        Vector3 playerPos;
        PlRay.SetinHand(2, this.gameObject);

        //playerPos = GameObject.Find("Player").transform.position;
        playerPos = transform.parent.transform.position;

        RB.isKinematic = true;
        this.transform.position = playerPos;

        this.transform.localPosition = Vector3.zero;
        this.transform.localPosition = PlayerAdjust;
        //wrench.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        this.transform.localRotation = Quaternion.Euler(-70.9f, -59.5f, 59.6f);
        this.transform.localScale = PlayerScale;
    }

    /*  philipsBolt
     * This Function resides in the philips Tool. 
     * When the Toolraycast strikes a Collider with a "hex" tag. This function is called, and the 
     * philips Tool becomes a child of the bolt, and attaches itself to it. (Adjusted)
     * While the function is "adjusted" if the player presses a mouse button, it will twist the bolt
     * and philips child it is supposed to. 
     * If the player moves the ToolRaycast off the "Bolt" collider, the philips snaps back to the person parent.
   
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

                this.transform.localRotation = Quaternion.Euler(360f, -180f, -203.8f);
                // Move the tool in position relative to the bolt
                this.transform.localPosition = philipsPosition;
                // Scale up
                this.transform.localScale = philipsScale;

                //Just in case, zero out the rotation of the tool object (Not the parent)

                //philips.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                //Not sure what's going on here???
                //philips.transform.parent = this.transform;
                //philips.transform.position = this.transform.position;                
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
                //philips.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                this.transform.localRotation = Quaternion.Euler(-70.9f, -59.5f, 59.6f);
                //philips.transform.parent = this.transform;
                //philips.transform.position = this.transform.position; 
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

