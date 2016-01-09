using UnityEngine;
using System.Collections;

public class BoltSpecs : MonoBehaviour 
{
    
    
    public float Travel = 0.15f; // How far the bolt has to travel until tight (from loose)
    public float ySpeed = .0001f; // The speed at which the bolt goes in or out.
    public bool Loose; // Is the bolt loose at the beginning
    public float Turned; // How far the bolt has turned

    public float LoosePos; // Where the bolt starts
    public float TightPos; // The tight position
    public float Current; //Current z position
    

    void Awake()
    {
        //All bolts (Fasteners) should start either completely loose, or all the way tight
        if (Loose)
        {
            LoosePos = this.transform.localPosition.y;
            TightPos = LoosePos - Travel;
            Current = LoosePos;
        }
        else
        {
            TightPos = this.transform.localPosition.y;
            LoosePos = TightPos + Travel;
            Current = TightPos;
        }

        Turned = 0.0f;
    }

    public void TurnBolt(bool direction, float turnSpeed)
    {
        Vector3 yS = (direction) ? new Vector3(0, -ySpeed, 0) : new Vector3(0, ySpeed, 0);
        
        // If tighten and it's not already tight, or loosen and it's not already loose
        if (((direction) && (Current >= TightPos)) || ((!direction) && (Current <= LoosePos)))
        {
            //If 
            this.transform.localPosition += yS;
            this.transform.Rotate(0, 0, turnSpeed);
            Current = this.transform.localPosition.y;
            Turned += turnSpeed;
            if ((Turned >= 360) || (Turned <= -360)) Turned = 0;
            if (Current >= LoosePos) Loose = true;
            else Loose = false;
        }       
    }
}
