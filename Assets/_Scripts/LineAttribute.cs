using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LineAttribute : PropertyAttribute 
{
    public float thickness = 2f;
    public LineAttribute(float thickness = 2f) { this.thickness = thickness; }
}