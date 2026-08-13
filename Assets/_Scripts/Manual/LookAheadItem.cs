using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class LookAheadItem : MonoBehaviour
{
    [Header("Visual")]
    public TextMeshProUGUI nameVisual;
    public TextMeshProUGUI pageVisual;

    [Header("Values")]
    public string name;
    public int pageId;

    public void Refresh()
    {
        nameVisual.text = name;
        pageVisual.text = pageId.ToString();
    }

    public void Choose()
    {
        LookAhead.instance.ChosePage(pageId);
    }
}