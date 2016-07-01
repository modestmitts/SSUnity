using System.Collections.Generic;
using UnityEngine;

public class PartDetach : MonoBehaviour 
{
    public List<GameObject> attachList;
    public bool Detached;
    private BoltSpecs boltstats;
    private Rigidbody rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
                
       foreach (Transform child in transform)
        {
            if (child.CompareTag("hex") || child.CompareTag("philips") || child.CompareTag("flathead"))
                attachList.Add(child.gameObject);
        }
            DetachCheck();
    }

    public void DetachCheck()
    {
        
        bool ls = true;
        foreach (GameObject go in attachList)
        {
            boltstats = go.GetComponent<BoltSpecs>();
            if (boltstats.Loose == false) ls = false;            
        }

        Detached = ls;

        if (Detached)
        {
            print("Turning on Gravity");

            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    
}