using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Market : Building, VillageBuildable
{
    private MarketSystem marketSystem;
    private TownStorage townStorage;

    [Line]
    [CenteredHeader("--- For Market ---", 20)]
    public GameObject marketWindow;
    public Slider[] sliders;
    public int[] storage = new int[5];
    public int[] sellMultiplyer = new int[5];
    public TextMeshProUGUI[] visual;
    public TextMeshProUGUI[] sellVisual;
    public TextMeshProUGUI sellValue;
    public Image[] arrows;
    public int[] price = new int[5];
    public int finalPrice;

    protected override void Start()
    {
        marketSystem = MarketSystem.instance;
        townStorage = TownStorage.instance;

        marketWindow = TownManager.instance.marketWindow;
        sliders = TownManager.instance.marketSliders;
        visual = TownManager.instance.marketVisuals;
        sellVisual = TownManager.instance.marketSellVisuals;
        arrows = TownManager.instance.marketArrows;
        sellValue = TownManager.instance.marketSellValue;

        TownManager.instance.availableMarket = this;
        if(!GridManager.instance.buildings.Contains(this))
        {
            GridManager.instance.buildings.Add(this);
        }
        HideVisuals();
    }

    protected override void Update()
    {
        bool isClick = Input.GetMouseButtonDown(0);
        bool isTouch = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;

        if ((isClick || isTouch) && !ViewMode.instance.viewMode)
        {
            if (EventSystem.current != null)
            {
                if (isClick && EventSystem.current.IsPointerOverGameObject() && isShowing) 
                {
                    return; 
                }
                if (isTouch && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && isShowing) 
                {
                    return; 
                }
            }

            if (CameraPinch.Instance != null && CameraPinch.Instance.IsPanning) 
            {
                return;
            }

            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (col == Physics2D.OverlapPoint(clickPos) && !isChoosing && !TownManager.instance.isBuilding && TownManager.instance.activeBuilding == null)
            {
                OnSpriteClicked();
            }
        }
    }

    protected override void OnSpriteClicked()
    {
        ShowVisuals();
    }

    public override void ShowVisuals()
    {
        foreach(var slider in sliders)
        {
            slider.value = 0;
        }

        UpdateVisuals();
        isShowing = true;
        marketWindow.SetActive(true);
    }

    public override void HideVisuals()
    {
        isShowing = false;
        marketWindow.SetActive(false);
    }

    public override void UpdateVisuals()
    {
        marketSystem.UpdateIconID();
        storage[0] = Mathf.FloorToInt(townStorage.wheat);
        storage[1] = townStorage.iron;
        storage[2] = townStorage.copper;
        storage[3] = townStorage.quartz;
        storage[4] = townStorage.titanium;

        for(int i = 0; i < sliders.Length; i++)
        {
            sliders[i].minValue = 0;
            sliders[i].maxValue = storage[i];
            visual[i].text = sliders[i].value.ToString();
            arrows[i].sprite = marketSystem.demandIcons[marketSystem.iconsId[i]].icon;
            int p = Mathf.RoundToInt(sliders[i].value * sellMultiplyer[i] * marketSystem.dailyDemand[i]);
            price[i] = p;
            sellVisual[i].text = "$" + price[i].ToString();
        }

        CalculateFinalPrice();
        sellValue.text = "$" + finalPrice.ToString();
    }

    public void UpdateSliderVisual(int id)
    {
        visual[id].text = sliders[id].value.ToString();
        int p = Mathf.RoundToInt(sliders[id].value * sellMultiplyer[id] * marketSystem.dailyDemand[id]);
        price[id] = p;
        sellVisual[id].text = "$" + price[id].ToString();
        CalculateFinalPrice();
        sellValue.text = "$" + finalPrice.ToString();
    }

    public void CalculateFinalPrice()
    {
        finalPrice = 0;

        foreach(var p in price)
        {
            finalPrice += p;
        }
    }

    public void Sell()
    {
        if(finalPrice == 0) return;

        CalculateFinalPrice();
        townStorage.wheat -= (float)sliders[0].value;
        townStorage.iron -= Mathf.RoundToInt(sliders[1].value);
        townStorage.copper -= Mathf.RoundToInt(sliders[2].value);
        townStorage.quartz -= Mathf.RoundToInt(sliders[3].value);
        townStorage.titanium -= Mathf.RoundToInt(sliders[4].value);
        townStorage.money += finalPrice;

        foreach(var slider in sliders)
        {
            slider.value = 0;
        }

        UpdateVisuals();
    }
}