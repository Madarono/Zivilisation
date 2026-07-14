using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LineAttribute : PropertyAttribute 
{
    public float thickness = 1f;
    public LineAttribute(float thickness = 1f) { this.thickness = thickness; }
}