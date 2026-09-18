using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildItem : MonoBehaviour
{
    [Header("Visuals")]
    public Image iconVisual;
    public TextMeshProUGUI priceVisual;
    public TextMeshProUGUI nameVisual;

    [Header("Data")]
    public int price;
    public int priceAfterDiscount;
    public string name;
    public Sprite icon;

    [Header("Building Data")]
    public GameObject itemPrefab;
    public Sprite itemSprite;
    public int width;
    public int height;

    [Header("Special Modifications")]
    public GameObject darkened;
    public bool onlyOne = false;
    public int onlyOneId = 0;

    void Start()
    {
        if(!BuildOptions.instance.items.Contains(this)) BuildOptions.instance.items.Add(this);

        Refresh();
    }

    public void Refresh()
    {
        priceAfterDiscount = Mathf.RoundToInt(price * BuildOptions.instance.priceValueMultiplyer);
        iconVisual.sprite = icon;
        priceVisual.text = "$" + priceAfterDiscount.ToString();
        nameVisual.text = name;

        if(!onlyOne) return;

        darkened.SetActive(BuildOptions.instance.CheckOnlyOne(onlyOneId));
    }

    public void Choose()
    {
        BuildOptions.instance.ChooseOption(this);
    }
}
