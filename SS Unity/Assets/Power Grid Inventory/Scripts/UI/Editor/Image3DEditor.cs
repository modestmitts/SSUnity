/**********************************************
* Power Grid Inventory
* Copyright 2015 James Clark
**********************************************/
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace AncientCraftGames.UI.Editor
{
    /// <summary>
    /// Custom inspector for the Image3D UI element.
    /// </summary>
    [CustomEditor(typeof(Image3D))]
    [CanEditMultipleObjects]
    public class Image3DEditor : UnityEditor.Editor
    {
        List<SerializedProperty> Props = new List<SerializedProperty>(5);
        Image3D Target;

        void OnEnable()
        {
            Target = this.target as Image3D;
            var EditorTargetType = typeof(Image3D);
            foreach(FieldInfo info in EditorTargetType.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.DeclaredOnly).Where (
				f => !Attribute.IsDefined(f, typeof(HideInInspector))) )
            {
                Props.Add(serializedObject.FindProperty(info.Name));
            }
        }


        /// <summary>
        /// Renders UI elements for the Image3D inspector view.
        /// </summary>
        public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
            
            EditorGUIUtility.wideMode = true;
            Target.Mesh = EditorGUILayout.ObjectField(new GUIContent("Mesh", "The 3D mesh to display within the confines of the RectTransform."), Target.Mesh, typeof(Mesh), true) as Mesh;
            Target.color = EditorGUILayout.ColorField(new GUIContent("Color", "Color tint of the icon."), Target.color);
            Target.material = EditorGUILayout.ObjectField(new GUIContent("Material", "The material applied to the model."), Target.material, typeof(Material), true) as Material;
            Target.PreserveAspect = EditorGUILayout.Toggle(new GUIContent("    Preserve Aspect", "If set, the mesh will maintain its original height-to-width aspect ratio when scaling."), Target.PreserveAspect);
            Target.Rotation = EditorGUILayout.Vector3Field(new GUIContent("Rotation", "The angle to display the model within the RectTransform. NOTE: Changing this value frequently can significantly impact performance when recalculating the mesh's verts."),
                                                          Target.Rotation);

            //this will handle any addition properties added to derived classes
            foreach (SerializedProperty prop in Props)
            {
                EditorGUILayout.PropertyField(prop, true);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(Target);
                serializedObject.ApplyModifiedProperties();
            }
        }
        
    }
}
