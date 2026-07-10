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
    public string name;
    public Sprite icon;

    [Header("Building Data")]
    public GameObject itemPrefab;
    public Sprite itemSprite;
    public int width;
    public int height;

    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        iconVisual.sprite = icon;
        priceVisual.text = "$" + price.ToString();
        nameVisual.text = name;
    }

    public void Choose()
    {
        BuildOptions.instance.ChooseOption(this);
    }
}
