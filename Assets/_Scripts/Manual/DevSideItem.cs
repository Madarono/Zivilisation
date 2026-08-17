using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DevSideItem : MonoBehaviour
{
    public DevSidePage page;
    public Image itself;
    public Sprite[] states;

    public void Select()
    {
        DevSideManual.instance.Select(this);
    }

    public void Deactivate()
    {
        itself.sprite = states[0];
    }

    public void Activate()
    {
        itself.sprite = states[1];
    }
}