using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingScanlines : MonoBehaviour
{
    public float scrollSpeed = 0.05f; 
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        Rect currentUV = rawImage.uvRect;

        if (Time.timeScale == 0 && TimeForward.instance.choosing != 0)
        {
            currentUV.y += scrollSpeed * Time.unscaledDeltaTime;
        }
        else
        {
            currentUV.y += scrollSpeed * Time.deltaTime;
        }

        rawImage.uvRect = currentUV;
    }
}