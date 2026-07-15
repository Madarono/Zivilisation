using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CenteredHeaderAttribute))]
public class CenteredHeaderDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        CenteredHeaderAttribute headerAttr = (CenteredHeaderAttribute)attribute;

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = headerAttr.fontSize; 

        position.yMin += 8; 

        EditorGUI.LabelField(position, headerAttr.headerText, style);
    }

    public override float GetHeight()
    {
        CenteredHeaderAttribute headerAttr = (CenteredHeaderAttribute)attribute;
        
        return headerAttr.fontSize + 18f; 
    }
}