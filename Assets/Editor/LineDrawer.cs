using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LineAttribute))]
public class LineDrawer : DecoratorDrawer 
{
    public override void OnGUI(Rect position) 
    {
        LineAttribute lineAttr = (LineAttribute)attribute;
        Rect rect = EditorGUI.IndentedRect(position);
        rect.y += rect.height * 0.5f;
        rect.height = lineAttr.thickness;
        EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f, 1f));
    }

    public override float GetHeight() 
    { 
        return 10f; 
    }
}