using System.Collections.Generic;
using UnityEngine;

public class PartDetach : MonoBehaviour 
{
    public List<GameObject> attachList;
    public bool Detached;
    private BoltSpecs boltstats;
    

    void Awake()
    {
       foreach (Transform child in transform)
        {
            if (child.CompareTag("bolt"))
                attachList.Add(child.gameObject);
        }
            DetachCheck();   
     
    }

    public void DetachCheck()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        bool ls = true;
        foreach (GameObject go in attachList)
        {
            boltstats = go.GetComponent<BoltSpecs>();
            if (boltstats.Loose == false) ls = false;            
        }

        Detached = ls;

        if (Detached)
        {
            print("Turnning on Gravity");
            
            rb.useGravity = true;
        }
        
    }

    
}
