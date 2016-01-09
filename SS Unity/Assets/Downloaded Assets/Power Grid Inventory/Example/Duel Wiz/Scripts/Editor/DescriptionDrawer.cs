using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(DescriptionAttribute))]
public class DescriptionDrawer : PropertyDrawer
{
    readonly int LabelHeight = 18;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(property.propertyType == SerializedPropertyType.String)
        {
            EditorGUI.PrefixLabel(position, new GUIContent(label));
            Rect r = new Rect(position.xMin, position.yMin + LabelHeight, position.width, position.height - LabelHeight);
            EditorGUI.BeginProperty(r, label, property);
            property.stringValue = EditorGUI.TextArea(r, property.stringValue);
            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use DescriptionAttribute with strings only.");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent content)
    {
        DescriptionAttribute attr = attribute as DescriptionAttribute;
        return base.GetPropertyHeight(property, content) * attr.Height + LabelHeight;
    }
}
