using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class AudioTextField : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private TMP_InputField inputField;
    private bool wasDeselected = false;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        // if (wasDeselected)
        // {
        OnSelectedAfterDeselect();
        // }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // wasDeselected = true;
    }

    private void OnSelectedAfterDeselect()
    {
        AudioManager.instance.Play(AudioManager.instance.textFieldSelect);
    }
}