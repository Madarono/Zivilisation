using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class MoneyCounter : MonoBehaviour
{
    public static MoneyCounter instance { get; private set; }
    private TownStorage storage;

    public RectTransform moneyWindow;
    public RectTransform differenceWindow;
    public TextMeshProUGUI moneyVisual;
    public TextMeshProUGUI differenceVisual;
    public Color[] differenceStates;

    [Header("Price Animation")]
    public int deltaPrice;
    public float getToPriceDuration = 1f;

    [Header("Show Animation")]
    public float openingDuration = 1f;
    public float stallDuration = 4f;
    public float closeDuration = 1f;
    public Vector2 stallDimensions;
    public Vector2 closedDimensions;

    [Header("Difference Animation")]
    public float difOpeningDuration;
    public float difStallDuration;
    public float difCloseDuration;
    public Vector2 difStallDimensions;
    public Vector2 difClosedDimensions;

    private Coroutine currentShow;
    private Coroutine currentUpdateMoney;
    private Coroutine currentDifference;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        storage = TownStorage.instance;
        deltaPrice = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Show();
        }
    }

    [ContextMenu("Show")]
    public void Show()
    {
        if (currentShow != null)
        {
            return;
        }

        currentShow = StartCoroutine(ShowMoney());
    }

    public void UpdateVisual()
    {
        moneyVisual.text = $"${storage.Money:N0}";
    }

    IEnumerator ShowMoney()
    {
        moneyVisual.gameObject.SetActive(true);
        float t = 0;
        moneyWindow.gameObject.SetActive(true);
        moneyWindow.sizeDelta = closedDimensions;

        int money = TownStorage.instance.Money;
        int difference = money - deltaPrice;

        if (currentUpdateMoney != null) StopCoroutine(currentUpdateMoney);
        currentUpdateMoney = StartCoroutine(UpdateMoney());

        while (t < openingDuration)
        {
            t += Time.unscaledDeltaTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t / openingDuration);
            moneyWindow.sizeDelta = Vector2.Lerp(closedDimensions, stallDimensions, smoothT);
            yield return null;
        }

        if (currentDifference != null) StopCoroutine(currentDifference);
        if (difference != 0) currentDifference = StartCoroutine(ShowDifference(difference));

        yield return new WaitForSecondsRealtime(stallDuration);
        moneyVisual.gameObject.SetActive(false);

        t = 0;
        while (t < closeDuration)
        {
            t += Time.unscaledDeltaTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t / closeDuration);
            moneyWindow.sizeDelta = Vector2.Lerp(stallDimensions, closedDimensions, smoothT);
            yield return null;
        }

        moneyWindow.sizeDelta = closedDimensions;
        moneyWindow.gameObject.SetActive(false);

        currentShow = null;
    }

    IEnumerator ShowDifference(int difference)
    {
        float t = 0;
        differenceWindow.gameObject.SetActive(true);
        differenceWindow.sizeDelta = difClosedDimensions;
        differenceVisual.gameObject.SetActive(true);

        int absDifference = Mathf.Abs(difference);
        differenceVisual.text = difference > 0 ? $"+${absDifference}" : $"-${absDifference}";
        differenceVisual.color = difference > 0 ? differenceStates[1] : differenceStates[0];

        while (t < difOpeningDuration)
        {
            t += Time.unscaledDeltaTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t / difOpeningDuration);
            differenceWindow.sizeDelta = Vector2.Lerp(difClosedDimensions, difStallDimensions, smoothT);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(difStallDuration);
        differenceVisual.gameObject.SetActive(false);

        t = 0;
        while (t < difCloseDuration)
        {
            t += Time.unscaledDeltaTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t / difCloseDuration);
            differenceWindow.sizeDelta = Vector2.Lerp(difStallDimensions, difClosedDimensions, smoothT);
            yield return null;
        }

        differenceWindow.sizeDelta = difClosedDimensions;
        differenceWindow.gameObject.SetActive(false);

        currentDifference = null;
    }

    IEnumerator UpdateMoney()
    {
        int money = TownStorage.instance.Money;
        int startPrice = deltaPrice;
        float t = 0;

        while (t < getToPriceDuration)
        {
            t += Time.unscaledDeltaTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t / getToPriceDuration);
            deltaPrice = Mathf.RoundToInt(Mathf.Lerp(startPrice, money, smoothT));
            moneyVisual.text = $"${deltaPrice:N0}";
            yield return null;
        }

        deltaPrice = money;
        moneyVisual.text = $"${deltaPrice:N0}";

        currentUpdateMoney = null;
    }
}