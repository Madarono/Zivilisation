using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraPinch : MonoBehaviour
{
    public static CameraPinch Instance { get; private set; }
    public Camera cam;
    public float zoomSpeed = 0.01f;
    public float panSpeed = 1.0f;
    public float maxSize = 50;
    public float minSize = 2f;
    public Vector3 islandPlace;
    public float cooldown = 3f;
    public float distanceTillReturn = 5f;
    public float cameraSpeed = 10f;

    [Header("Desktop specific")]
    public float scrollSpeed = 40f;

    private Vector3 panStartWorldPos;
    private bool isPanning = false;
    public bool IsPanning => isPanning; //Read-Only for other scripts
    private Coroutine returnCoroutine;

    void Awake()
    {
        Instance = this;
        if (cam == null) 
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        if ((Time.timeScale == 0 && TimeForward.instance.choosing != 0) || RoadSystem.instance.isMultiBrush) 
        {
            return;
        }

        bool interacted = false;

        if (Input.touchCount == 2)
        {
            isPanning = false;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

            float prevMag = (prevTouch0 - prevTouch1).magnitude;
            float currMag = (touch0.position - touch1.position).magnitude;

            float delta = currMag - prevMag;
            HandleZoom(delta);
            interacted = true;
        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId) && !ViewMode.instance.viewMode)
                {
                    isPanning = false;
                }
                else
                {
                    isPanning = true;
                    interacted = true;
                    ResetReturnTimer();
                }
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                float factor = ((cam.orthographicSize * 2f) / Screen.height) * panSpeed;
                Vector3 dragDelta = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, 0) * factor;

                cam.transform.position -= dragDelta;
                interacted = true;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isPanning = false;
            }
        }
        else if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1) && !ViewMode.instance.viewMode)
                {
                    isPanning = false;
                }
                else
                {
                    panStartWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                    isPanning = true;
                    interacted = true;
                    ResetReturnTimer();
                }
            }
            else if (Input.GetMouseButton(0) && isPanning)
            {
                Vector3 currentWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector3 direction = panStartWorldPos - currentWorldPos;
                direction.z = 0;
                cam.transform.position += direction;
                interacted = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isPanning = false;
            }

            float scrollDelta = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scrollDelta) > 0f)
            {
                HandleZoom(scrollDelta * scrollSpeed);
                interacted = true;
            }
        }

        if (interacted)
        {
            if (!isPanning && Input.touchCount == 0)
            {
                ManageReturnRoutine();
            }
        }
    }

    public void HandleZoom(float delta)
    {
        if (Mathf.Abs(delta) < 0.01f) return;

        ResetReturnTimer();

        if (cam.orthographic)
        {
            float sizeBefore = cam.orthographicSize;
            float zoomStep = delta * zoomSpeed * (sizeBefore / maxSize);
            cam.orthographicSize = Mathf.Clamp(sizeBefore - zoomStep, minSize, maxSize);
        }
        else
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - delta * zoomSpeed, 15f, 100f);
        }
        
        ManageReturnRoutine();
    }

    private void ResetReturnTimer()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    private void ManageReturnRoutine()
    {
        ResetReturnTimer();

        Vector3 camPosCheck = cam.transform.position;
        camPosCheck.z = islandPlace.z;

        if (Vector3.Distance(camPosCheck, islandPlace) >= distanceTillReturn)
        {
            returnCoroutine = StartCoroutine(ReturnToIslandAfterDelay(cooldown));
        }
    }

    IEnumerator ReturnToIslandAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 targetPos = islandPlace;
        targetPos.z = cam.transform.position.z;

        while (Vector3.Distance(cam.transform.position, targetPos) > 0.01f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, Time.unscaledDeltaTime * cameraSpeed);
            yield return null;
        }

        cam.transform.position = targetPos;
        returnCoroutine = null;
    }

    public void ChangeZoom(float amount)
    {
        HandleZoom(amount);
    }
}