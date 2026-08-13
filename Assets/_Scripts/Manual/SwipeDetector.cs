using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class SwipeDetector : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    public float swipeThreshold = 50f;

    private TMP_InputField inputField;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool isSwiping = false;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = eventData.position;
        isSwiping = false;

        if (inputField != null)
        {
            inputField.selectionAnchorPosition = inputField.caretPosition;
            inputField.selectionFocusPosition = inputField.caretPosition;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isSwiping = true;

        if (inputField != null)
        {
            inputField.DeactivateInputField();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inputField != null && inputField.isFocused)
        {
            inputField.DeactivateInputField();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        endPosition = eventData.position;
        Vector2 swipeVector = endPosition - startPosition;

        if (Mathf.Abs(swipeVector.x) >= swipeThreshold)
        {
            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                if (swipeVector.x < 0)
                {
                    if (ManualSystem.instance != null) ManualSystem.instance.NextPage();
                }
                else
                {
                    if (ManualSystem.instance != null) ManualSystem.instance.PrevPage();
                }
            }
        }
    }
}