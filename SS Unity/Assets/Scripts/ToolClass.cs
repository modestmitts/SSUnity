using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/************************************************************************
 *                    Tool Class                                        *
 *                    
 * This is the generic script for equippable tools. This script should
 * handle the following
 * ID: What tool is being held
 * Positions: Where are tool will be positioned if it is held, or when 
 * it is being used.
 * Tool Performance Stats: Store information on how the tool does it's job
 * Parent: This will store the parent, or non-parent of the tool.
 * *********************************************************************/


public class ToolClass : MonoBehaviour 
{
    public GameObject tool; // This is the GameObject model
    public string tool_name; // This is the name of the tool
    public float perf_spec; // Performance Specification, how well the tool does its job
    public List<Vector3> position; // Position 0 should always be the hand held position. 
    public List<Quaternion> rotation; // Rotation 0, should be the hand held rotation.
    //public string //I want the icon path here, or some reference to the sprite slug (Although not sure this is necessary, considering the way the inventory system and Prefabs work
    /*
    public ToolClass()
    {
        tool_name = null;
        position = null;
        rotation = null;
    }

    public ToolClass(string tname, List<Vector3> posList, List<Quaternion> rotList)
    {
        tool_name = tname;
        position = posList;
        rotation = rotList;
    }

    public Vector3 PosAtNum(int num)
    {
        if (position[num] != null) return position[num];
        else return Vector3.zero;    
    }


    public Quaternion RotAtNum(int num)
    {
        if (rotation[num] != null) return rotation[num];
        else return null;
    }
   */

}
