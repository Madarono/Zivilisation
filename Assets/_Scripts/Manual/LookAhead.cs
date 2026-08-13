using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LookAhead : MonoBehaviour
{
    public static LookAhead instance {get; private set;}
    private ManualSystem manual;

    public GameObject window;
    public bool isOpen;

    [Header("Look Ahead")]
    public TMP_InputField searchBar;
    public GameObject searchItemPrefab;
    public Transform parent;

    private List<string> availableHeaders = new List<string>();
    private List<int> availablePageNumbers = new List<int>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        manual = ManualSystem.instance;
        CloseWindow();
    }
    
    public void BothWindow()
    {
        isOpen = !isOpen;

        if(isOpen) OpenWindow();
        else CloseWindow();
    }

    public void OpenWindow()
    {
        window.SetActive(true);
        isOpen = true;
        ClearList();
    }

    public void CloseWindow()
    {
        window.SetActive(false);
        isOpen = false;
    }

    public void UpdateList()
    {
        if(parent.childCount > 0)
        {
            foreach(Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }

        availableHeaders.Clear();
        availablePageNumbers.Clear();

        string header = searchBar.text;

        if(int.TryParse(header, out int headerInt))
        {
            PopulateListInt(headerInt);
        }
        else
        {
            PopulateListString(header);
        }

        if(availableHeaders.Count == 0) return;

        for(int i = 0; i < availableHeaders.Count; i++)
        {
            GameObject go = Instantiate(searchItemPrefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            go.transform.position = parent.position;

            if(go.TryGetComponent(out LookAheadItem goScript))
            {
                goScript.name = availableHeaders[i].Length > 0 ? availableHeaders[i] : "No header";
                goScript.pageId = availablePageNumbers[i];
                goScript.Refresh();
            }
        }
    }

    void PopulateListString(string headerStr)
    {
        for(int i = 0; i < manual.headerInfo.Count; i++)
        {
            if (manual.headerInfo[i].Contains(headerStr, System.StringComparison.OrdinalIgnoreCase))
            {
                availableHeaders.Add(manual.headerInfo[i]);
                availablePageNumbers.Add(i);
            }
        }
    }

    void PopulateListInt(int headerInt)
    {
        if (headerInt < 0 || headerInt >= manual.headerInfo.Count) return; //Page too high or not invalid

        availableHeaders.Add(manual.headerInfo[headerInt]);
        availablePageNumbers.Add(headerInt);
    }

    public void ClearList()
    {
        searchBar.text = "";
        if(parent.childCount > 0)
        {
            foreach(Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void ChosePage(int pageId)
    {
        if(isLeftPage(pageId))
        {
            manual.leftPageId = pageId;
            manual.rightPageId = pageId + 1;
        }
        else
        {
            manual.leftPageId = pageId - 1;
            manual.rightPageId = pageId;
        }

        while(manual.pageInfo.Count <= manual.rightPageId)
        {
            manual.pageInfo.Add("");
            manual.headerInfo.Add("");
        }

        manual.UpdatePageContent();
        CloseWindow();
    }

    bool isLeftPage(int pageId)
    {
        return pageId % 2 == 0;
    }
}