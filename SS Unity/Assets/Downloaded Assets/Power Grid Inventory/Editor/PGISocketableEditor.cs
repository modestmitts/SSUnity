/**********************************************
* Power Grid Inventory
* Copyright 2015 James Clark
**********************************************/
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace PowerGridInventory.Editor
{
    [CustomEditor(typeof(Socketable))]
    [CanEditMultipleObjects]
    public class PGISocketableEditor : PGIAbstractEditor
    {

        protected override void OnEnable()
        {
            //we need to do this because CustomEditor is kinda dumb
            //and won't expose the type we passed to it. Plus relfection
            //can't seem to get at the data either.
            EditorTargetType = typeof(Socketable);
            base.OnEnable();
        }

    }
}
