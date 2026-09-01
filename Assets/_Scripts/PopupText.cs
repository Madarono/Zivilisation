using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public static PopupText instance {get; private set;}

    public TextMeshProUGUI popupVisual;
    private Coroutine currentPopup;

    [Header("Audio")]
    public float amplification = 0.75f;

    [Header("Animation")]
    public float duration;
    public float stayFullDuration = 3f;

    [Header("Debug")]
    public string input;

    private GameObject currentSound;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        popupVisual.gameObject.SetActive(false);
    }

    public void Popup(string input)
    {
        if(currentPopup != null)
        {
            StopCoroutine(currentPopup);
        }

        popupVisual.text = input;
        if(currentSound != null) Destroy(currentSound);
        currentSound = AudioManager.instance.PlayGameObject(AudioManager.instance.popupText, amplification);

        currentPopup = StartCoroutine(DoPopupText());
    }

    IEnumerator DoPopupText()
    {
        popupVisual.gameObject.SetActive(true);

        Color c = popupVisual.color;

        float t = 0f;
        while (t < duration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
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
            t += Time.unscaledDeltaTime;
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
