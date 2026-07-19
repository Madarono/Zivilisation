using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class CameraPinch : MonoBehaviour
{
    public static CameraPinch Instance { get; private set; }
    public Camera cam;
    public float panSpeed = 1.0f;
    public Vector3 islandPlace;
    public float cooldown = 3f;
    public float distanceTillReturn = 5f;
    public float cameraSpeed = 10f;

    [Header("Pixel Perfect Hardcoded Resolutions")]
    public int zoomedOutX = 320;
    public int zoomedOutY = 180;
    public int zoomedInX = 160;
    public int zoomedInY = 90;
    public float zoomDuration = 1f;
    [Tooltip("Prevents the player from spam-triggering resolution swaps.")]
    public float zoomCooldown = 0.4f; 

    [Header("UI Buffer Transition Mask")]
    [SerializeField] private CanvasGroup uiMask;

    private Vector3 panStartWorldPos;
    private bool isPanning = false;
    public bool IsPanning => isPanning;
    private Coroutine returnCoroutine;
    private Coroutine zoomCoroutine; 
    private PixelPerfectCamera pixelCam;
    private float nextZoomAllowedTime = 0f;

    void Awake()
    {
        Instance = this;
        if (cam == null) 
        {
            cam = Camera.main;
        }

        if (cam != null)
        {
            pixelCam = cam.GetComponent<PixelPerfectCamera>();
        }

        if (uiMask != null) uiMask.alpha = 0f;
    }

    void Update()
    {
        if ((Time.timeScale == 0 && TimeForward.instance.choosing != 0) || RoadSystem.instance.isMultiBrush) 
        {
            return;
        }

        if (Input.touchCount == 2)
        {
            isPanning = false;

            if (Time.unscaledTime < nextZoomAllowedTime)
            {
                return;
            }

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);


            Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

            float prevMag = (prevTouch0 - prevTouch1).magnitude;
            float currMag = (touch0.position - touch1.position).magnitude;

            float delta = currMag - prevMag;

            if (Mathf.Abs(delta) > 0.5f)
            {
                if (delta > 0f)
                {
                    SetSnapZoom(true);
                }
                else
                {
                    SetSnapZoom(false);
                }
            }
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
                    ResetReturnTimer();
                }
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                float factor = ((cam.orthographicSize * 2f) / Screen.height) * panSpeed;
                Vector3 dragDelta = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y, 0) * factor;

                Vector3 newPos = cam.transform.position - dragDelta;
                cam.transform.position = GetSnappedPosition(newPos);
                ResetReturnTimer();
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isPanning = false;
                ManageReturnRoutine();
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
                    ResetReturnTimer();
                }
            }
            else if (Input.GetMouseButton(0) && isPanning)
            {
                Vector3 currentWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector3 direction = panStartWorldPos - currentWorldPos;
                direction.z = 0;
                
                Vector3 newPos = cam.transform.position + direction;
                cam.transform.position = GetSnappedPosition(newPos);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isPanning = false;
                ManageReturnRoutine();
            }
        }
    }

    public void SetSnapZoom(bool isZoomedIn)
    {
        if (pixelCam == null) return;

        if (Time.unscaledTime < nextZoomAllowedTime) return;

        ResetReturnTimer();

        int targetX = isZoomedIn ? zoomedInX : zoomedOutX;
        int targetY = isZoomedIn ? zoomedInY : zoomedOutY;

        if (IsAlreadyZoomed(targetX, targetY))
        {
            if (Input.touchCount == 0 && !isPanning)
            {
                ManageReturnRoutine();
            }
            return;
        }

        // Lock future inputs immediately
        nextZoomAllowedTime = Time.unscaledTime + zoomCooldown;

        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ExecuteZoomSwap(isZoomedIn, targetX, targetY));
    }

    private IEnumerator ExecuteZoomSwap(bool isZoomedIn, int targetX, int targetY)
    {
        if (uiMask != null) uiMask.alpha = 1f;

        yield return new WaitForEndOfFrame();

        pixelCam.refResolutionX = targetX;
        pixelCam.refResolutionY = targetY;

        if (isZoomedIn)
        {
            LensDistortWarp.instance.Enlarge();
        }
        else
        {
            LensDistortWarp.instance.Small();
        }

        yield return null;
        yield return null;
        
        if (uiMask != null) uiMask.alpha = 0f;
        
        ManageReturnRoutine();
        zoomCoroutine = null;
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
            Vector3 newPos = Vector3.Lerp(cam.transform.position, targetPos, Time.unscaledDeltaTime * cameraSpeed);
            cam.transform.position = GetSnappedPosition(newPos);
            yield return null;
        }

        cam.transform.position = GetSnappedPosition(targetPos);
        returnCoroutine = null;
    }

    bool IsAlreadyZoomed(int zoomX, int zoomY)
    {
        return pixelCam.refResolutionX == zoomX && pixelCam.refResolutionY == zoomY;
    }

    private Vector3 GetSnappedPosition(Vector3 position)
    {
        float ppu = 8f;
        position.x = Mathf.Round(position.x * ppu) / ppu;
        position.y = Mathf.Round(position.y * ppu) / ppu;
        return position;
    }
}