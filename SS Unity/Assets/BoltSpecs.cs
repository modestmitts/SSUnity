using UnityEngine;
using System.Collections;

public class BoltSpecs : MonoBehaviour 
{
    public float Current; //Current ratio of 
    public float LoosePos; // Where the bolt starts
    public float Travel = 0.15f;
    public bool Loose;
    

    void Awake()
    {
        //All bolts should start either completely loose, or tightened all the way
        //
        LoosePos = this.transform.localPosition.z;

        if (Loose)
        {
            Current = 1.0f;
        }
        else
        {
            Current = 0.0f;
            LoosePos -= Travel; 
        }
    }

    void Update()
    { 
        if (used)
        {
            Current = this.transform.localPosition.z / LoosePos;
        }
    }
}
