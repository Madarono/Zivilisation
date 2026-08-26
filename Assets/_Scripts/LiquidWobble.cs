using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LiquidWobble : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.05f;
    public float floatSpeed = 1.5f;
    public float floatAmount = 2f;

    private Vector3 baseScale;
    private Vector3 basePosition;
    private float timeOffset;

    void Start()
    {
        baseScale = transform.localScale;
        basePosition = transform.localPosition;
        timeOffset = Random.Range(0f, 100f); 
    }

    void Update()
    {
        float time = Time.time + timeOffset;

        float scaleX = Mathf.Sin(time * pulseSpeed) * pulseAmount;
        float scaleY = Mathf.Cos(time * pulseSpeed * 1.2f) * pulseAmount;
        transform.localScale = baseScale + new Vector3(scaleX, scaleY, 0f);

        float posX = Mathf.Sin(time * floatSpeed) * floatAmount;
        float posY = Mathf.Cos(time * floatSpeed * 0.8f) * floatAmount;
        transform.localPosition = basePosition + new Vector3(posX, posY, 0f);
    }
}