using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveSystem : MonoBehaviour
{
    public static ObjectiveSystem instance { get; private set; }
    public GameObject window;
    public RectTransform windowRect; // Cache RectTransform directly to avoid repeated GetComponent calls
    public bool isOpen;
    public Image button;
    public Sprite[] buttonStates;

    [Header("Window Animation")]
    public float windowSpeed = 500f; // Units/pixels per second
    public Vector2 windowClosed;
    public Vector2 windowOpen;

    private Coroutine currentAnim;

    [Header("Objectives")]
    public GameObject objectivePrefab;
    public Transform parent;
    public TextMeshProUGUI[] availableObjectives;
    public ObjectiveItem[] availableItems;

    void Awake()
    {
        instance = this;
        if (windowRect == null && window != null)
        {
            windowRect = window.GetComponent<RectTransform>();
        }
    }

    void Start()
    {
        CloseWindow();
    }

    public void BothWindow()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            OpenWindow();
        }
        else
        {
            CloseWindow();
        }
    }

    public void OpenWindow()
    {
        isOpen = true;
        window.SetActive(true);
        UpdateVisuals();
    }

    public void CloseWindow()
    {
        isOpen = false;
        UpdateVisuals();
        window.SetActive(false);
    }

    void UpdateVisuals()
    {
        if (button != null && buttonStates.Length >= 2)
        {
            button.sprite = isOpen ? buttonStates[1] : buttonStates[0];
        }

        UpdateObjectives();
    }

    public void UpdateObjectives()
    {
        List<string> visual = new List<string>();
        List<TutorialStep> steps = new List<TutorialStep>();

        for(int i = 0; i < TutorialSystem.instance.objectives.Length; i++)
        {
            if(!TutorialSystem.instance.objectives[i].done)
            {
                visual.Add(TutorialSystem.instance.objectives[i].visual);
                steps.Add(TutorialSystem.instance.objectives[i].step);
            }

            if(visual.Count >= availableObjectives.Length) break;
        }

        foreach(var objective in availableObjectives)
        {
            objective.gameObject.SetActive(false);
        }

        for(int i = 0; i < visual.Count; i++)
        {
            availableObjectives[i].text = visual[i];
            availableItems[i].step = steps[i];
            availableObjectives[i].gameObject.SetActive(true);
        }
    }
}