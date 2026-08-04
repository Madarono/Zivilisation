using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ActiveWindow : MonoBehaviour
{
    public static ActiveWindow instance {get; private set;}
    public Window currentActiveWindow;
    public bool isActive;
    public bool briefActive;

    void Awake()
    {
        instance = this;
    }
}