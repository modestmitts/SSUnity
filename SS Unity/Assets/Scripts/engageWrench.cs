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
    private Vector3 BoltAdjust = new Vector3(-0.012f, -0.075f, 0.0075f);
    private Vector3 PlayerAdjust = new Vector3(0.283f, 0.386f, 0.64f);
        

    private bool adjusted = false;
    public void wrenchBolt(GameObject newParent, bool work)
    {
        
        if (work)
        {
            if (!adjusted)
            {
                transform.parent = newParent.transform;
                this.transform.localPosition = Vector3.zero;
                this.transform.localPosition = BoltAdjust;
                wrench.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                this.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                wrench.transform.parent = this.transform;
                wrench.transform.position = this.transform.position;
                adjusted = true;
            }

        }
        else 
        {
            if (adjusted)
            {
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


}
