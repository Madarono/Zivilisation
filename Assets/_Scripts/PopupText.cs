using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class PopupText : MonoBehaviour
{
    public static PopupText instance { get; private set; }

    public TextMeshProUGUI popupVisual;
    public TextMeshProUGUI miniPopupVisual;
    
    private Coroutine currentPopup;
    private Coroutine currentMiniPopup;
    private Coroutine currentStopMiniPopup;

    [Header("Audio")]
    public float amplification = 0.75f;

    [Header("Animation")]
    public float duration = 0.5f;
    public float stayFullDuration = 3f;

    [Header("Mini Animation")]
    public float miniDuration = 0.25f;
    public float miniStayFullDuration = 1f;

    [Header("Mini Popup")]
    public RectTransform miniPopupRect;
    public RectTransform canvasRect;
    public float baseCameraSize = 5f;
    private Camera cam;

    [Header("Debug")]
    public string input;

    private GameObject currentSound;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (popupVisual != null) popupVisual.gameObject.SetActive(false);
        if (miniPopupVisual != null) miniPopupVisual.gameObject.SetActive(false);
        
        cam = Camera.main;

        if (miniPopupRect != null)
        {
            miniPopupRect.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    public void Popup(string input, bool sound = true)
    {
        if (currentPopup != null)
        {
            StopCoroutine(currentPopup);
        }

        popupVisual.text = input;

        if (currentSound != null && sound) Destroy(currentSound);
        if (sound) currentSound = AudioManager.instance.PlayGameObject(AudioManager.instance.popupText, amplification);

        currentPopup = StartCoroutine(DoPopupText(popupVisual));
    }
    
    public void MiniPopup(string input, Transform origin, Vector3? worldOffset = null)
    {
        if (currentMiniPopup != null)
        {
            StopCoroutine(currentMiniPopup);
        }

        miniPopupVisual.text = input;

        Vector3 offset = worldOffset ?? new Vector3(0, 1.2f, 0);

        currentMiniPopup = StartCoroutine(DoMiniPopupProcess(miniPopupVisual, origin, offset));
    }

    public void StopMiniPopup()
    {
        if(currentMiniPopup != null) 
        {
            StopCoroutine(currentMiniPopup);
            if(currentStopMiniPopup != null) return;

            currentStopMiniPopup = StartCoroutine(StopMiniPopupProcess(miniPopupVisual));
        }
    }

    IEnumerator DoMiniPopupProcess(TextMeshProUGUI visual, Transform origin, Vector3 offset)
    {
        UpdateMiniPopupPosition(origin, offset);

        float t = 0f;
        Color c = visual.color;
        visual.gameObject.SetActive(true);

        while (t < miniDuration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / (miniDuration * 0.5f));
            visual.color = c;

            UpdateMiniPopupPosition(origin, offset);
            UpdateMiniPopupSize();
            yield return null;
        }

        c.a = 1f;
        visual.color = c;

        float elapsed = 0f;
        while (elapsed < miniStayFullDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            UpdateMiniPopupPosition(origin, offset);
            UpdateMiniPopupSize();
            yield return null;
        }

        t = 0f;
        while (t < miniDuration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / (miniDuration * 0.5f));
            visual.color = c;

            UpdateMiniPopupPosition(origin, offset);
            UpdateMiniPopupSize();
            yield return null;
        }

        c.a = 0f;
        visual.color = c;
        visual.gameObject.SetActive(false);

        currentMiniPopup = null;
    }

    IEnumerator StopMiniPopupProcess(TextMeshProUGUI visual)
    {
        float t = 0f;
        Color c = visual.color;
    
        t = 0f;
        while (t < miniDuration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / (miniDuration * 0.5f));
            visual.color = c;

            // UpdateMiniPopupPosition(origin, offset);
            UpdateMiniPopupSize();
            yield return null;
        }

        c.a = 0f;
        visual.color = c;
        visual.gameObject.SetActive(false);

        currentStopMiniPopup = null;
    }

    void UpdateMiniPopupPosition(Transform origin, Vector3 offset)
    {
        if (origin == null || miniPopupRect == null || canvasRect == null) return;

        if (cam == null) cam = Camera.main;

        Vector3 screenPoint = cam.WorldToScreenPoint(origin.position + offset);

        if (screenPoint.z < 0)
        {
            miniPopupVisual.enabled = false;
            return;
        }

        if (!miniPopupVisual.enabled) miniPopupVisual.enabled = true;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out Vector2 localPoint))
        {
            miniPopupRect.anchoredPosition = localPoint;
        }
    }

    void UpdateMiniPopupSize()
    {
        if (cam == null || miniPopupRect == null) return;

        float scaleFactor = baseCameraSize / cam.orthographicSize;
        miniPopupRect.localScale = Vector3.one * scaleFactor;
    }

    IEnumerator DoPopupText(TextMeshProUGUI visual)
    {
        visual.gameObject.SetActive(true);
        Color c = visual.color;

        float t = 0f;
        while (t < duration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / (duration * 0.5f));
            visual.color = c;
            yield return null;
        }

        c.a = 1f;
        visual.color = c;

        yield return new WaitForSecondsRealtime(stayFullDuration);

        t = 0f;
        while (t < duration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / (duration * 0.5f));
            visual.color = c;
            yield return null;
        }

        c.a = 0f;
        visual.color = c;
        visual.gameObject.SetActive(false);

        currentPopup = null;
    }
}