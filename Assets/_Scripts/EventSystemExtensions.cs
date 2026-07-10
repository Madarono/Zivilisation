using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public static class EventSystemExtensions
{
    public static bool IsPointerOverLayer(LayerMask layerMask, int pointerId = -1)
    {
        if(EventSystem.current == null)
        {
            return false;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        
        if(Application.isMobilePlatform && Input.touchCount > 0)
        {
            int id = pointerId >= 0 ? pointerId : 0;

            if(id < Input.touchCount)
            {
                eventData.position = Input.GetTouch(id).position;
            }
            else
            {
                return false;
            }
        }
        else
        {
            eventData.position = Input.mousePosition;
        }

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach(RaycastResult result in results)
        {
            if(((1 << result.gameObject.layer) & layerMask) != 0)
            {
                return true;
            }
        }

        return false;
    }
}
