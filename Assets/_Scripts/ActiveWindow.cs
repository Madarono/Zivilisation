using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ActiveWindow : MonoBehaviour
{
    public static ActiveWindow instance {get; private set;}
    public Window currentActiveWindow;
    public bool isActive;

    void Awake()
    {
        instance = this;
    }
}