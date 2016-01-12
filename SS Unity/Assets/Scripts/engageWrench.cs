using UnityEngine;
using System.Collections;

public class engageWrench : MonoBehaviour 
{
    // Player Position Adjustment
    //Position(0.36f, 0.301f, 0.762f)
    //Rotation(0, 180,  340)
    //Scale (0.5, 0.5, 0.5)

    // Bolt Position Adjustment
    //Position(-0.012, -0.07499, 0.0056)
    //Rotation(0, 90,  0)
    //Scale (0.0333, 0.0333, 0.0333)

    public GameObject wrench;
    public float turnSpeed = 0.15f;

    // These positions are for attaching the wrench to the different faces on the bolt
    private Vector3[] wrenchPosition = new[] { new Vector3(-0.7f, -3.2f, 1.00f), // Position0 (Tight)
                                                new Vector3(2.45f, -2.25f, 1.00f), // Position1 (Loose)
                                                new Vector3(3.15f, 1.0f, 1.00f), // Position2  
                                                new Vector3(0.74f, 3.23f, 1.00f), // Position3 
                                                new Vector3(-2.45f, 2.25f, 1.00f), // Position4 
                                                new Vector3(-3.15f, -1.0f, 1.00f) }; // Position5


       private float[] wrenchTurn = new[] { 347.6f, 47.6f, 107.6f, 167.6f, 227.7f, 287.6f };
    
    private Vector3 PlayerAdjust = new Vector3(0.36f, 0.301f, 0.762f); 
   
   

    private bool adjusted = false;
    private bool ready = false;
    private BoltSpecs boltstats;
    private int currentPos;

    public void wrenchBolt(GameObject newParent, bool work)
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

                // If the bolt is Tight, set tool to the Tight position (Position 0)
                if (boltstats.Loose) currentPos = 1;
                // Else set it to the Loose position (Position 1) (I might change this to Position 2 )
                else currentPos = 0;
                print("Current Position " + currentPos);
                // Rotate the tool in position relative to the bolt
                this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, wrenchTurn[currentPos]);
                // Move the tool in position relative to the bolt
                this.transform.localPosition = wrenchPosition[currentPos];

                //Just in case, zero out the rotation of the tool object (Not the parent)
                
                //wrench.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                //Not sure what's going on here???
                //wrench.transform.parent = this.transform;
                //wrench.transform.position = this.transform.position;                
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
                //wrench.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                this.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 340.0f);
                //wrench.transform.parent = this.transform;
                //wrench.transform.position = this.transform.position;   
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
                currentPos = ChangePosition();
            }
            if (Input.GetMouseButton(0)) 
            {
                boltstats.TurnBolt(!tighten, -turnSpeed);
                currentPos = ChangePosition();
            }
            
        }    
    }

    int ChangePosition()
    {
        int pos = Mathf.Abs((int)boltstats.Turned / 60);
        if (pos != currentPos)
        {            
            Debug.Log("Current Position" + pos);
            this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, wrenchTurn[currentPos]);
            this.transform.localPosition = wrenchPosition[currentPos];
            currentPos = pos;
        }
        return currentPos;
    }

    
}
