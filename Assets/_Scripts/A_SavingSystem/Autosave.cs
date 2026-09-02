using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Autosave : MonoBehaviour
{
    public static Autosave instance { get; private set; }
    
    private float lastSaveTime;
    private const float saveCooldown = 1.0f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            TriggerAutoSave();
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            TriggerAutoSave();
        }
    }

    public void TriggerAutoSave()
    {
        if (Time.unscaledTime - lastSaveTime < saveCooldown) return;

        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.SaveGame();
            lastSaveTime = Time.unscaledTime;
        }
    }
}