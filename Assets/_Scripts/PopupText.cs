using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public static PopupText instance {get; private set;}

    public TextMeshProUGUI popupVisual;
    private Coroutine currentPopup;

    [Header("Animation")]
    public float duration;
    public float stayFullDuration = 3f;

    [Header("Debug")]
    public string input;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        popupVisual.gameObject.SetActive(false);
    }

    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.P))
    //     {
    //         Popup(input);
    //     }
    // }

    public void Popup(string input)
    {
        if(currentPopup != null)
        {
            StopCoroutine(currentPopup);
        }

        popupVisual.text = input;

        currentPopup = StartCoroutine(DoPopupText());
    }

    IEnumerator DoPopupText()
    {
        popupVisual.gameObject.SetActive(true);

        Color c = popupVisual.color;

        float t = 0f;
        while (t < duration * 0.5f)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / (duration * 0.5f));

            c.a = alpha;
            popupVisual.color = c;

            yield return null;
        }

        c.a = 1f;
        popupVisual.color = c;

        yield return new WaitForSeconds(stayFullDuration);

        t = 0f;
        while (t < duration * 0.5f)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / (duration * 0.5f));

            c.a = alpha;
            popupVisual.color = c;

            yield return null;
        }

        c.a = 0f;
        popupVisual.color = c;

        popupVisual.gameObject.SetActive(false);
    }
}
