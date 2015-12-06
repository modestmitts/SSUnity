/**********************************************
* Power Grid Inventory
* Copyright 2015 James Clark
**********************************************/
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using PowerGridInventory.Utility;

namespace PowerGridInventory.Editor
{
	
	/// <summary>
	/// Abstract base Editor class that can be derived from for model, view, slot, and item editors.
	/// It facilitates easy setup of folding for any events marked with the proper PGIFoldedEvent attribute.
	/// </summary>
	public abstract class PGIAbstractEditor : UnityEditor.Editor
	{
		List<SerializedProperty> Props = new List<SerializedProperty>(5);
		List<SerializedProperty> Events = new List<SerializedProperty>(5);
		SerializedProperty EventFolder;
		protected Type EditorTargetType = null;

		protected virtual void OnEnable()
		{
			if(EditorTargetType == null) throw new UnityException("EditorTargetType was not set for a derived class.");

			//Gah! Reflection simply doesn't work with the CustomEditor class I guess!
			/*
			 * CustomEditor attr = Attribute.GetCustomAttribute(this.GetType(), typeof(CustomEditor), true) as CustomEditor;
			if(attr == null) throw new UnityException("This class must have the CustomEditor attribute.");
			//HACK: For some dumb reason, CustomEditor does not expose the inspected type so we need
			//to look for it using reflection.
			Type inspectedType = null;
			FieldInfo property = attr.GetType().GetField("m_InspectedType", BindingFlags.NonPublic | BindingFlags.Instance);
			if(property != null)
			{
				var propVal = property.GetValue(attr);
				inspectedType = propVal.GetType();
				Debug.Log ("Inspected Type: " + inspectedType.Name);

			}
			else throw new UnityException("Could not retrived the property 'm_InspectedType' from the CustomEditor class. Most likely, this member has changed with an update to Unity.");
			*/

			bool foldFlagFound = false;
			foreach(FieldInfo info in EditorTargetType.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly).Where (
				f => !Attribute.IsDefined(f, typeof(HideInInspector))) )
			{
				//info.GetCustomAttributes(true);
				if(Attribute.IsDefined(info, typeof(PGIFoldedEventAttribute)))
				{
					if(serializedObject.FindProperty(info.Name) == null)
					{
						Debug.LogError("Could not locate the event: " + info.Name);
					}
					else Events.Add(serializedObject.FindProperty(info.Name));
				}
				else if(Attribute.IsDefined(info, typeof(PGIFoldFlagAttribute)))
				{
					if(foldFlagFound)
					{
						throw new UnityException("There can only be one field with the PGIFoldFlagAttrribute within a class.");
					}
					if(!info.FieldType.IsAssignableFrom(typeof(bool)))
					{
						throw new UnityException("PGIFoldFlagAttribute is only valid on a field of type bool.");
					}
					EventFolder = serializedObject.FindProperty(info.Name);
				}
				else
				{
					if(serializedObject.FindProperty(info.Name) == null)
					{
						Debug.LogError("Could not locate the field: " + info.Name);
					}
					else Props.Add(serializedObject.FindProperty(info.Name));
				}
			}

		}

        public virtual void OnSubInspectorGUI()
        {

        }

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

            //render override-specific properties
            OnSubInspectorGUI();

			//Display non-event properties normally.
			foreach(SerializedProperty prop in Props)
			{
				EditorGUILayout.PropertyField(prop, true);
			}

            GUILayout.Space(10);
			//display events. Try to fold them if possible.
			if(EventFolder == null)
			{
				//no folder flag was provided. Display events as normal.
				foreach(SerializedProperty prop in Events)
				{
					EditorGUILayout.PropertyField(prop, true);
				}
			}
			else
			{
				//There was a folder flag declared.
				//Fold the events using it.
                EventFolder.boolValue = EditorGUILayout.Foldout(EventFolder.boolValue, new GUIContent("Events"));
				if(EventFolder.boolValue)
				{
					foreach(SerializedProperty prop in Events)
					{
						EditorGUILayout.PropertyField(prop, true);
					}
				}
			}
			
			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
			EditorGUIUtility.LookLikeControls();
		}
	}
}
