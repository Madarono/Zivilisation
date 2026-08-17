using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevSideManual : MonoBehaviour
{
    public static DevSideManual instance {get; private set;}
    public DevSidePage currentPage;
    public GameObject iconPrefab;

    public TextMeshProUGUI leftPage;
    public TextMeshProUGUI rightPage;

    public List<GameObject> icons = new List<GameObject>();

    //Values of stretch
    private RectTransform lpRect;
    private float lpLeft;
    private float lpBottom;
    private float lpRight;
    private float lpTop;

    private RectTransform rpRect;
    private float rpLeft;
    private float rpBottom;
    private float rpRight;
    private float rpTop;
    
    [Header("Layouts - Middle Up")]
    public Transform[] middleUp;
    public float[] middlePadding = new float[4];

    [Line]
    [Header("Layouts - Right")]
    public Transform[] right;
    public float[] rightPadding = new float[4];

    [Line]
    [Header("Layouts - Left")]
    public Transform[] left;
    public float[] leftPadding = new float[4];

    [Line]
    [Header("Layouts - Middle Down")]
    public Transform[] middleDown;
    public float[] middleDownPadding = new float[4];


    [Header("Manuals")]
    public DevSideItem[] manuals;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if(leftPage.gameObject.TryGetComponent(out RectTransform lpRect))
        {
            this.lpRect = lpRect;
            lpLeft = lpRect.offsetMin.x;
            lpBottom = lpRect.offsetMin.y;
            lpRight = -lpRect.offsetMax.x;
            lpTop = -lpRect.offsetMax.y;
        }
        if(rightPage.gameObject.TryGetComponent(out RectTransform rpRect))
        {
            this.rpRect = rpRect;
            rpLeft = rpRect.offsetMin.x;
            rpBottom = rpRect.offsetMin.y;
            rpRight = -rpRect.offsetMax.x;
            rpTop = -rpRect.offsetMax.y;
        }

        Select(manuals[0]);
    }

    public void LoadPage()
    {
        if(icons.Count > 0)
        {
            for(int i = icons.Count - 1; i >= 0; i--)
            {
                Destroy(icons[i]);
            }

            icons.Clear();
        }
        leftPage.text = currentPage.leftPageInfo;
        PadPage(lpRect, false);
        SetIcons(false);
        AdjustLayerGroup(false);
        

        rightPage.text = currentPage.rightPageInfo;
        PadPage(rpRect, true);
        SetIcons(true);
        AdjustLayerGroup(true);
    }

    void PadPage(RectTransform page, bool right)
    {
        float[] padding = new float[4];

        ImageLayoutType layout = right ? currentPage.rightPageLayout : currentPage.leftPageLayout;

        switch (layout)
        {
            case ImageLayoutType.MiddleUp:
                    padding = middlePadding;
                break;

            case ImageLayoutType.Right:
                    padding = this.rightPadding;
                break;

            case ImageLayoutType.Left:
                    padding = this.leftPadding;
                break;
            
            case ImageLayoutType.MiddleDown:
                    padding = middleDownPadding;
                break;

            case ImageLayoutType.None:
                break;

            default:
                break;
        }

        float leftPadding = padding[0];
        float rightPadding = padding[1];
        float topPadding = padding[2];
        float bottomPadding = padding[3];

        float currentLeft = right ? rpLeft : lpLeft;
        float currentRight = right ? rpRight : lpRight;
        float currentTop = right ? rpTop : lpTop;
        float currentBottom = right ? rpBottom : lpBottom;

        page.offsetMin = new Vector2(currentLeft + leftPadding, currentBottom + bottomPadding);
        page.offsetMax = new Vector2(-(currentRight + rightPadding), -(currentTop + topPadding));
    }

    void SetIcons(bool right)
    {
        Sprite[] pageIcons = right ? currentPage.rightPageIcons : currentPage.leftPageIcons;

        if(pageIcons.Length > 0)
        {
            Transform parent = ParentLayout(right);
            if(parent == null) return;

            int width = right ? currentPage.rightPageWidth : currentPage.leftPageWidth;
            int height = right ? currentPage.rightPageHeight : currentPage.leftPageHeight;

            for(int i = 0; i < pageIcons.Length; i++)
            {
                GameObject go = Instantiate(iconPrefab, parent);
                go.transform.localScale = Vector3.one;
                if(go.TryGetComponent(out Image goIcon))
                {
                    goIcon.sprite = pageIcons[i];
                    ResizeIcon(goIcon, width, height);
                    icons.Add(go);
                }
            }
        }
    }

    void AdjustLayerGroup(bool right)
    {
        Transform parent = ParentLayout(right);
        if(parent == null) return;

        int spacing = right ? currentPage.rightPageSpacing : currentPage.leftPageSpacing;
        RectOffset padding = right ? currentPage.rightPagePadding : currentPage.leftPagePadding;

        if (parent.TryGetComponent(out HorizontalOrVerticalLayoutGroup layoutGroup))
        {
            layoutGroup.spacing = spacing;
            layoutGroup.padding = new RectOffset(padding.left, padding.right, padding.top, padding.bottom);
            
            if (parent is RectTransform rectTransform)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }
    }

    Transform ParentLayout(bool right)
    {
        ImageLayoutType layout = right ? currentPage.rightPageLayout : currentPage.leftPageLayout;

        switch(layout)
        {
            case ImageLayoutType.MiddleUp:
                return middleUp[right ? 1 : 0];
                break;

            case ImageLayoutType.Right:
                return this.right[right ? 1 : 0];
                break;

            case ImageLayoutType.Left:
                return left[right ? 1 : 0];
                break;
            
            case ImageLayoutType.MiddleDown:
                return middleDown[right ? 1 : 0];
                break;

            case ImageLayoutType.None:
                break;

            default:
                break;
        }

        return null;
    }

    void ResizeIcon(Image icon, int width, int height)
    {
        RectTransform rect = icon.rectTransform;
        rect.sizeDelta = new Vector2(width, height);
    }

    public void Select(DevSideItem item)
    {
        foreach(var manual in manuals)
        {
            manual.Deactivate();
        }

        item.Activate();
        currentPage = item.page;
        LoadPage();
    }
}