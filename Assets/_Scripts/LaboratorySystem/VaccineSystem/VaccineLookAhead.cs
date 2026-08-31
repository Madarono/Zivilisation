using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class VaccineLookAhead : MonoBehaviour
{
    [Header("Look Ahead")]
    public TMP_InputField searchBar;
    public GameObject vaccineItemPrefab;
    public Transform parent;

    private List<string> availableNames = new List<string>();
    private List<int> availableVirusId = new List<int>();

    public void UpdateList()
    {
        if(parent.childCount > 0)
        {
            foreach(Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }

        availableNames.Clear();
        availableVirusId.Clear();

        string header = searchBar.text;
        PopulateListString(header);

        if(availableNames.Count == 0) return;

        for(int i = 0; i < availableNames.Count; i++)
        {
            GameObject go = Instantiate(vaccineItemPrefab, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            go.transform.position = parent.position;

            if(go.TryGetComponent(out VaccineItem goScript))
            {
                goScript.name = availableNames[i].Length > 0 ? availableNames[i] : "...";
                goScript.virusId = availableVirusId[i];
                goScript.Refresh();
            }
        }
    }

    public void PopulateListString(string headerStr)
    {
        for(int i = 0; i < LaboratorySystem.instance.virusNames.Count; i++)
        {
            if (LaboratorySystem.instance.virusNames[i].Contains(headerStr, System.StringComparison.OrdinalIgnoreCase) && VaccineSystem.instance.curedVirusId.Contains(i))
            {
                availableNames.Add(LaboratorySystem.instance.virusNames[i]);
                availableVirusId.Add(i);
            }
        }
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
}