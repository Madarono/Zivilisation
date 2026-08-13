using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ManualTextFormatter : MonoBehaviour
{
    public TMP_InputField pageField;
    public int h1SizePer = 180;
    public int h2SizePer = 140;
    public int h3SizePer = 115;
    public int pSizePer = 100;

    public void OnEnable()
    {
        if (pageField != null)
        {
            pageField.onSelect.AddListener(OnInputFieldSelected);
        }
    }

    public void OnDisable()
    {
        if (pageField != null)
        {
            pageField.onSelect.RemoveListener(OnInputFieldSelected);
        }
    }

    public void SetPageField(TMP_InputField newPageField)
    {
        UnsubscribeField(pageField);
        StopAllCoroutines();

        pageField = newPageField;
        SubscribeField(pageField);
    }

    private void SubscribeField(TMP_InputField field)
    {
        if (field != null)
        {
            field.onSelect.AddListener(OnInputFieldSelected);
        }
    }

    private void UnsubscribeField(TMP_InputField field)
    {
        if (field != null)
        {
            field.onSelect.RemoveListener(OnInputFieldSelected);
        }
    }

    private void OnInputFieldSelected(string text)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCaretToEndRoutine());
    }

    public void FormatBold()
    {
        ApplyTagToSelection("<b>");
    }

    public void StopFormatBold()
    {
        ApplyTagToSelection("</b>");
    }

    public void FormatItalic()
    {
        ApplyTagToSelection("<i>");
    }

    public void StopFormatItalic()
    {
        ApplyTagToSelection("</i>");
    }

    public void FormatH1()
    {
        ApplyTagToSelection($"<size={h1SizePer}%><b>");
    }

    public void FormatH2()
    {
        ApplyTagToSelection($"<size={h2SizePer}%><b>");
    }

    public void FormatH3()
    {
        ApplyTagToSelection($"<size={h3SizePer}%><b>");
    }

    public void StopFormatH()
    {
        ApplyTagToSelection("</b></size>");
    }

    public void FormatParagraph()
    {
        ApplyTagToSelection($"<size={pSizePer}%>");
    }

    public void StopFormatParagraph()
    {
        ApplyTagToSelection("</size>");
    }

    public void ApplyTagToSelection(string format)
    {
        if (pageField == null) return;

        if (!pageField.isFocused)
        {
            pageField.Select();
            pageField.ActivateInputField();
        }

        pageField.text += format;
        pageField.ForceLabelUpdate();
        StopAllCoroutines();
        StartCoroutine(SetCaretToEndNextFrame());
    }

    private IEnumerator SetCaretToEndNextFrame()
    {
        yield return new WaitForEndOfFrame();

        pageField.Select();
        pageField.ActivateInputField();

        int endPos = pageField.text.Length;
        pageField.stringPosition = endPos;
        pageField.caretPosition = endPos;
    }

    private IEnumerator MoveCaretToEndRoutine()
    {
        yield return new WaitForEndOfFrame();

        if (pageField != null)
        {
            pageField.ActivateInputField();

            int endPos = pageField.text.Length;
            pageField.stringPosition = endPos;
            pageField.caretPosition = endPos;
        }
    }
}