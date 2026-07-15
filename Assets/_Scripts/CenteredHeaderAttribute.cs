using UnityEngine;

public class CenteredHeaderAttribute : PropertyAttribute
{
    public string headerText;
    public int fontSize;

    public CenteredHeaderAttribute(string headerText, int fontSize = 12)
    {
        this.headerText = headerText;
        this.fontSize = fontSize;
    }
}