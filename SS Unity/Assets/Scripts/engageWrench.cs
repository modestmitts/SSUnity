using UnityEngine;
using System.Collections;

public class engageWrench : MonoBehaviour 
{
    // Player Position Adjustment
    //Position(0.245, 0.53, 0.88)
    //Rotation(0, -34,  0)
    //Scale (0.1, 0.1, 0.1)

    // Bolt Position Adjustment
    //Position(-0.012, -0.07499, 0.0056)
    //Rotation(0, 90,  0)
    //Scale (0.0333, 0.0333, 0.0333)
    public GameObject wrench;
    public float turnSpeed = 0.15f;

    private Vector3[] wrenchPosition = new[] { new Vector3(-0.012f, -0.075f, 0.006f), // Position0 (Tight)
                                                new Vector3(0.0565f, -0.05f, 0.006f), // Position1 (Loose)
                                                new Vector3(0.07f, 0.027f, 0.006f), // Position2  
                                                new Vector3(0.012f, 0.075f, 0.006f), // Position3 
                                                new Vector3(-0.0565f, 0.05f, 0.006f), // Position4 
                                                new Vector3(-0.07f, -0.027f, 0.006f) }; // Position5

    private float [] wrenchTurn = new[]{0.0f, -60.0f, -120.0f, -180.0f, -240.0f, -300.0f};
    
    private Vector3 PlayerAdjust = new Vector3(0.283f, 0.386f, 0.64f); 
   
   

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
                boltstats = transform.parent.GetComponent(typeof(BoltSpecs)) as BoltSpecs; 
                transform.parent = newParent.transform;
                this.transform.localPosition = Vector3.zero;            

                if (!boltstats.Loose) currentPos = 0;
                else currentPos = 1;
                
                this.transform.localRotation = Quaternion.Euler(wrenchTurn[currentPos], 90.0f, 0.0f);
                this.transform.localPosition = wrenchPosition[currentPos];
                wrench.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                wrench.transform.parent = this.transform;
                wrench.transform.position = this.transform.position;                
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
                this.transform.localPosition = Vector3.zero;
                this.transform.localPosition = PlayerAdjust;
                wrench.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                this.transform.localRotation = Quaternion.Euler(0.0f, -34.0f, 0.0f);
                wrench.transform.parent = this.transform;
                wrench.transform.position = this.transform.position;   
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
            if (Input.GetMouseButton(1)) boltstats.TurnBolt(tighten, turnSpeed);                               
            if (Input.GetMouseButton(0)) boltstats.TurnBolt(!tighten, -turnSpeed);           
   
            if (((int)boltstats.Turned / 60) != currentPos) 
            {
               currentPos = Mathf.Abs((int)boltstats.Turned / 60);
               Debug.Log("Current Position" + currentPos);
               this.transform.localRotation = Quaternion.Euler(wrenchTurn[currentPos], 90.0f, 0.0f);
               this.transform.localPosition = wrenchPosition[currentPos];
               currentPos = 1;
            }
        }    
    }
    
}
