using UnityEngine;

public class ToolRaycast : MonoBehaviour 
{
   // public Camera camera;
    public float toolReachDistance;
    public GameObject ToolParent;
   
    //public float sightDistance; //Or whatever distance
    private engageWrench wrenchScript;
    private bool engageTool;
    private float currentDistance;



    void Start()
    {
        engageTool = false;
        currentDistance = toolReachDistance;
        wrenchScript = ToolParent.GetComponent(typeof(engageWrench)) as engageWrench;      
    }

    void Update()
    {
        RaycastHit hit;        
        Vector3 forward = transform.TransformDirection(Vector3.forward * currentDistance);
        Ray ray = new Ray(transform.position, forward);

        Debug.DrawRay(transform.position, forward, Color.black, 0f, true);

        if (Physics.Raycast(ray, out hit, currentDistance) && (hit.collider.tag == "bolt"))
        {
            if (!engageTool)
            {
                wrenchScript.wrenchBolt(hit.collider.transform.gameObject, true);
                engageTool = true;
            }     
        }
        else if (engageTool)
        {
            wrenchScript.wrenchBolt(transform.parent.gameObject, false);
            engageTool = false;
        }


    }





}
