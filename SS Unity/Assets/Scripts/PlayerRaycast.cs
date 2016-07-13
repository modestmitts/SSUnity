using UnityEngine;
using System.Collections;

/*         Player Raycast

 This is the major script that handles player interaction with objects in the game. 
  It is only meant to identify what the player is clicking, and call the function within the object accordingly
 There should be no functionality within this script
 
 * This will also keep track of what the player is currently equipped with as well
*/


public class PlayerRaycast : MonoBehaviour
{
    public enum equipped { none, wrench, flathead_driver, philips_driver };
    public equipped inHand;    
   // public Camera camera;
    public float toolReachDistance;
    public string HitTag;
        
    private float currentDistance;
    private DoorControl DC;

    private engageWrench eWR;
    private engagePhilips ePH;
    private engageFlathead eFH;

    private bool engageTool;

    private PartDetach pd;

    void Start()
    {
        currentDistance = toolReachDistance;
        inHand = equipped.none;
        eWR = null;
        ePH = null;
        eFH = null;
  
    }

    void Update()
    {
        RaycastHit hit;
        //Vector3 armlength = transform.TransformDirection(Vector3.forward * currentDistance);
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        Ray ray = new Ray(transform.position, forward);
        

        // Things that happen only at arm's distance
        if (Physics.Raycast(ray, out hit))
        {
            HitTag = hit.collider.tag;
            if (Input.GetMouseButtonDown(0))
            {
                // Click on a Door or Panel 
                if (((hit.collider.tag == "panel1") || (hit.collider.tag == "panel2")) && (hit.distance < currentDistance))
                {
                    DoorControl DC = hit.collider.gameObject.GetComponent<DoorControl>();
                    // If the DoorControl script isn't found with the mesh, try the parent
                    if (DC == null) DC = hit.collider.transform.parent.GetComponent<DoorControl>();

                    if (DC) DC.ToggleDoor();
                    else print("WARNING: Door/Panel does not have a findable DoorControl script");

                }

               
                if ((hit.collider.tag == "Pickup Item") && (hit.distance < currentDistance))
                {
                    hit.collider.gameObject.GetComponent<MyPickup>().RaycastPickup();
                }

                // If Raycast finds a ship's Access Icon, and is clicked on, it should open the ship's details panel
                if (hit.collider.tag == "Access_Icon")
                {                   
                   hit.collider.gameObject.GetComponentInParent<StampTrigger>().OpenInterface();
                   
                }
            }
            
            // If Raycast finds a Hex bolt while you have the Wrench equipped
            if ((((hit.collider.tag == "hex") && (inHand == equipped.wrench)) || 
                ((hit.collider.tag == "flathead") && (inHand == equipped.flathead_driver)) ||
                ((hit.collider.tag == "philips") && (inHand == equipped.philips_driver))) && 
                (hit.distance < currentDistance))
            {
                if (!engageTool)
                {
                    /// How are we accessing wrenchscript here?
                    if (inHand == equipped.wrench)   eWR.engageBolt(hit.collider.transform.gameObject, true);
                    if (inHand == equipped.flathead_driver)  eFH.engageBolt(hit.collider.transform.gameObject, true);
                    if (inHand == equipped.philips_driver)  ePH.engageBolt(hit.collider.transform.gameObject, true);
                    engageTool = true;
                }
            }
            else if (engageTool)   
            {
                if (inHand == equipped.wrench)   eWR.engageBolt(transform.parent.gameObject, false);
                if (inHand == equipped.flathead_driver)  eFH.engageBolt(transform.parent.gameObject, false);
                if (inHand == equipped.philips_driver)  ePH.engageBolt(transform.parent.gameObject, false);
                engageTool = false;
            }


        }
    }


    public void SetinHand(int num, GameObject tool)
    {
        eWR = null;
        ePH = null;
        eFH = null;
  
        switch (num)
        {
            case 0:
                {
                    inHand = equipped.none;
                    break;
                }
            case 1:
                {
                    inHand = equipped.flathead_driver;
                    eFH = this.transform.parent.GetComponentInChildren<engageFlathead>();
                    break;
                }
            case 2:
                {
                    inHand = equipped.philips_driver;
                    ePH = this.transform.parent.GetComponentInChildren<engagePhilips>();
                    break;
                }
            case 3:
                {
                    inHand = equipped.wrench;
                    eWR = tool.GetComponent<engageWrench>();
                    break;
                }
            default:
                {
                    inHand = equipped.none;
                    break;
                }
        }

     }
}
