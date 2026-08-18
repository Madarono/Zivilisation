using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum FormatVisualType
{
    Visual,
    Size
}

public enum FormatType
{
    Bold,
    Italic,
    H1,
    H2,
    H3,
    P
}

[System.Serializable]
public class FormatButton 
{
    public Image button;
    public Sprite[] buttonStates;
    public FormatType type;
    public FormatVisualType visualType;
    public bool isActive;
}

public class ManualSystem : MonoBehaviour
{
    public static ManualSystem instance {get; private set;}
    public ManualTextFormatter formatter;

    [Header("Visual")]
    public FormatButton[] formatButtons;
    private int previousSizeId = -1;

    [Header("Pages")]
    public TMP_InputField[] pages;
    public TMP_InputField[] headers;

    [Header("Page info")]
    public List<string> pageInfo = new List<string>();
    public List<string> headerInfo = new List<string>();
    public int leftPageId; //4
    public int rightPageId; //5
    public TextMeshProUGUI leftPageVisual;
    public TextMeshProUGUI rightPageVisual;
    public GameObject prevPageButton;

    void Awake()
    {
        instance = this;
    }
    
    //Pages
    public void ReturnToFirstPages()
    {
        leftPageId = 0;
        rightPageId = 1;

        UpdatePageContent();
    }

    public void MakeInitialPages()
    {
        if(pageInfo.Count >= 2 && headerInfo.Count >= 2) return;

        while(pageInfo.Count < 2 && headerInfo.Count < 2)
        {
            pageInfo.Add("");
            headerInfo.Add("");
        }
    }

    public void NextPage()
    {
        rightPageId += 2;
        leftPageId += 2;

        while(pageInfo.Count <= rightPageId && headerInfo.Count <= rightPageId) //If making a whole entire new two pages
        {
            pageInfo.Add("");
            headerInfo.Add("");
        }

        UpdatePageContent();
    }

    public void PrevPage()
    {
        if(leftPageId <= 0)
        {
            leftPageId = 0;
            rightPageId = 1;
            return;
        }

        rightPageId -= 2;
        leftPageId -= 2;
        UpdatePageContent();
    }

    public void MakeNewPage()
    {
        leftPageId = pageInfo.Count;
        rightPageId = leftPageId + 1;

        pageInfo.Add("");
        pageInfo.Add("");
        headerInfo.Add("");
        headerInfo.Add("");

        UpdatePageContent();
    }
    public void UpdatePageContent()
    {
        pages[0].text = pageInfo[leftPageId];
        pages[1].text = pageInfo[rightPageId];
        headers[0].text = headerInfo[leftPageId];
        headers[1].text = headerInfo[rightPageId];

        leftPageVisual.text = leftPageId.ToString();
        rightPageVisual.text = rightPageId.ToString();

        prevPageButton.SetActive(leftPageId != 0);
    }

    public void UpdatePageInfo() //This is for the list
    {
        pageInfo[leftPageId] = pages[0].text;
        pageInfo[rightPageId] = pages[1].text;
        headerInfo[leftPageId] = headers[0].text;
        headerInfo[rightPageId] = headers[1].text;
    }
    
    public void SelectPage(int id)
    {
        if(formatter.pageField == pages[id]) return;

        if (formatter.pageField != null)
        {
            formatter.pageField.selectionAnchorPosition = formatter.pageField.text.Length;
            formatter.pageField.selectionFocusPosition = formatter.pageField.text.Length;
            formatter.pageField.DeactivateInputField();
        }

        formatter.SetPageField(pages[id]);

        pages[id].Select();
        pages[id].ActivateInputField();
        pages[id].caretPosition = pages[id].text.Length;
        pages[id].selectionAnchorPosition = pages[id].text.Length;
        pages[id].selectionFocusPosition = pages[id].text.Length;
    }
    
    // //Formatting
    // public void FormatButton(int id)
    // {
    //     if(formatButtons[id].isActive) //we clicked on it again
    //     {
    //         DeselectCurrent(id);
    //         return;
    //     }
        
    //     DeselectPrevious(id);

    //     if(formatButtons[id].visualType == FormatVisualType.Size) previousSizeId = id;

    //     formatButtons[id].button.sprite = formatButtons[id].buttonStates[1];
    //     formatButtons[id].isActive = true;

    //     SendFormat(formatButtons[id].type);
    // }

    // public void DeselectAllButtons()
    // {
    //     foreach(var formatButton in formatButtons)
    //     {
    //         formatButton.button.sprite = formatButton.buttonStates[0];
    //         formatButton.isActive = false;
    //     }

    //     previousSizeId = -1;
    // }

    // public void DeselectCurrent(int id)
    // {
    //     formatButtons[id].button.sprite = formatButtons[id].buttonStates[0];
    //     formatButtons[id].isActive = false;
    //     StopFormat(formatButtons[id].type);

    //     if(formatButtons[id].visualType == FormatVisualType.Size) previousSizeId = -1;
    // }

    // public void DeselectPrevious(int id)
    // {
    //     int previousId = formatButtons[id].visualType == FormatVisualType.Size ? previousSizeId : -1; //-1 js to end it early

    //     if(previousId == -1) return;

    //     formatButtons[previousId].button.sprite = formatButtons[previousId].buttonStates[0];
    //     formatButtons[previousId].isActive = false;
    //     StopFormat(formatButtons[previousId].type);
    // }

    // public void SendFormat(FormatType type)
    // {
    //     switch(type)
    //     {
    //         case FormatType.Bold:
    //             formatter.FormatBold();
    //             break;

    //         case FormatType.Italic:
    //             formatter.FormatItalic();
    //             break;

    //         case FormatType.H1:
    //             formatter.FormatH1();
    //             break;

    //         case FormatType.H2:
    //             formatter.FormatH2();
    //             break;
            
    //         case FormatType.H3:
    //             formatter.FormatH3();
    //             break;

    //         case FormatType.P:
    //             formatter.FormatParagraph();
    //             break;
    //     }
    // }

    // public void StopFormat(FormatType type)
    // {
    //     switch(type)
    //     {
    //         case FormatType.Bold:
    //             formatter.StopFormatBold();
    //             break;

    //         case FormatType.Italic:
    //             formatter.StopFormatItalic();
    //             break;

    //         case FormatType.H1:
    //             formatter.StopFormatH();
    //             break;

    //         case FormatType.H2:
    //             formatter.StopFormatH();
    //             break;
            
    //         case FormatType.H3:
    //             formatter.StopFormatH();
    //             break;

    //         case FormatType.P:
    //             formatter.StopFormatParagraph();
    //             break;
    //     }
    // }
}