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

    [Header("Sound")]
    public float stepPercent = 0.02f;
    public float minAmplification = 0.75f;
    public float maxAmplification = 1.15f;
    public float minSoundCooldown = 0.04f;

    private float stepInterval;
    private float lastPlayedValue;
    private float lastPlayTime;
    private float maxExpectedSpeed;

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
        bool increase = (money - deltaPrice) > 0;

        lastPlayedValue = startPrice;
        lastPlayTime = Time.unscaledTime;

        CalculateStepInterval(startPrice, money);

        float t = 0;

        while (t < getToPriceDuration)
        {
            t += Time.unscaledDeltaTime;
            float smoothT = Mathf.SmoothStep(0f, 1f, t / getToPriceDuration);
            deltaPrice = Mathf.RoundToInt(Mathf.Lerp(startPrice, money, smoothT));
            
            TrySound(increase);

            moneyVisual.text = $"${deltaPrice:N0}";
            yield return null;
        }

        deltaPrice = money;
        moneyVisual.text = $"${deltaPrice:N0}";

        currentUpdateMoney = null;
    }

    void CalculateStepInterval(int startPrice, int money)
    {
        float range = Mathf.Abs(money - startPrice);
        stepInterval = Mathf.Max(1f, range * stepPercent);

        float avgSpeed = range / Mathf.Max(0.01f, getToPriceDuration);
        maxExpectedSpeed = avgSpeed * 1.5f;
    }

    void TrySound(bool increase)
    {
        int currentValue = deltaPrice;
        float valueDelta = Mathf.Abs(currentValue - lastPlayedValue);
        float timeDelta = Time.unscaledTime - lastPlayTime;

        if (valueDelta >= stepInterval && timeDelta >= minSoundCooldown)
        {
            float currentSpeed = valueDelta / timeDelta;
            float speedRatio = Mathf.Clamp01(currentSpeed / Mathf.Max(1f, maxExpectedSpeed));
            float amplification = Mathf.Lerp(minAmplification, maxAmplification, speedRatio);

            var audioClip = increase ? AudioManager.instance.moneyIncrease : AudioManager.instance.moneyDecrease;
            AudioManager.instance.Play(audioClip, amplification);

            lastPlayedValue = currentValue;
            lastPlayTime = Time.unscaledTime;
        }
    }
}