using UnityEngine;
using System.Collections;

public class DescriptionAttribute : PropertyAttribute
{
    public float Height;

    public DescriptionAttribute(int lines)
    {
        this.Height = lines;
    }
}
