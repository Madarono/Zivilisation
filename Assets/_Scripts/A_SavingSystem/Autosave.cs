using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Autosave : MonoBehaviour
{
    public static Autosave instance { get; private set; }
    
    private float lastSaveTime;
    private const float saveCooldown = 1.0f;
    private bool isSaving = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            ForceLifecycleSave();
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            ForceLifecycleSave();
        }
    }

    private void OnApplicationQuit()
    {
        ForceLifecycleSave();
    }

    public void TriggerAutoSave()
    {
        if (Time.unscaledTime - lastSaveTime < saveCooldown) return;
        ExecuteSave();
    }

    private void ForceLifecycleSave()
    {
        if (isSaving) return; 

        ExecuteSave();
    }

    private void ExecuteSave()
    {
        if (DataPersistenceManager.instance != null)
        {
            isSaving = true;
            DataPersistenceManager.instance.SaveGame();
            lastSaveTime = Time.unscaledTime;
            isSaving = false;
        }
    }
}