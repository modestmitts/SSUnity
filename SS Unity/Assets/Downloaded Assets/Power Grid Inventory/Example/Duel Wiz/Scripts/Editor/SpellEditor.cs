using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// We need this so that GUILayout will work for poeprty drawers.
/// </summary>
[CustomEditor(typeof(Spell))]
public class SpellEditor : Editor
{

}
