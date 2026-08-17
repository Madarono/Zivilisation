using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum ImageLayoutType
{
    None,
    MiddleUp,
    Right,
    Left,
    MiddleDown
}

[CreateAssetMenu(fileName = "New Page", menuName = "FeverFall/ Dev_Side_Page")]
public class DevSidePage : ScriptableObject
{
    [Header("Left Page")]
    [TextArea(10, 16)]public string leftPageInfo;

    [Header("Left Page Images")]
    public ImageLayoutType leftPageLayout;
    public Sprite[] leftPageIcons;
    public int leftPageWidth = 100;
    public int leftPageHeight = 100;
    public int leftPageSpacing = 10;
    public RectOffset leftPagePadding = new RectOffset();
    
    [Line]
    [Header("Right Page")]
    [TextArea(10, 16)]public string rightPageInfo;

    [Header("Right Page Images")]
    public ImageLayoutType rightPageLayout;
    public Sprite[] rightPageIcons;
    public int rightPageWidth = 100;
    public int rightPageHeight = 100;
    public int rightPageSpacing = 10;
    public RectOffset rightPagePadding = new RectOffset();
}