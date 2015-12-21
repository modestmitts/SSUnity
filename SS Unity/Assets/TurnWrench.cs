using UnityEngine;
using System.Collections;
/***************************************************************
 * This function will be called if the Engage Wrench sets the 
 * Ready bool. 
 * 
 * If the user presses and holds the Left Mouse the Wrench and 
 * Bolt should spin Left (Loosen), conversely if the user 
 * presses the Right Mouse, it should spin right (Tighten).
 * 
 * In addition, if the user detargets the bolt, then the Ready
 * bool will be set false, and spin should cease. 
 **************************************************************/ 


public class TurnWrench : MonoBehaviour {
    public bool Ready;
    public float SpinAmount;
    public float TLAmount;
    public float LooseMax;
    public float TightMax;
	
	// Update is called once per frame
	void Update () 
    {
        if (Ready)
        {
            if (Input.GetMouseButtonDown(1))
            {
                // Rotate Parent Bolt around Z Axis by Added Spin Amount
                // Move Parent Bolt away by TLAmount to a Max

            }

            

            //Detect Left Mouse
            //Rotate Parent Bolt around Z Axis by Subtracting Spin Amount
        }
	}

    void SetReady(bool rdy)
    {
        Ready = rdy;
    }
}
