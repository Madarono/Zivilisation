using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Autosave : MonoBehaviour
{
    //For Desktop (Windows/Mac) and WebGL windows
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Debug.Log("App moved to BACKGROUND (Lost Focus). Saving game state...");
            TriggerAutoSave();
        }
        else
        {
            Debug.Log("App moved to FOREGROUND (Gained Focus). Pause menus can open here.");
        }
    }

    //For Mobile devices (iOS/Android) 
    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            Debug.Log("App suspended to BACKGROUND (Paused). Saving game state...");
            TriggerAutoSave();
        }
        else
        {
            Debug.Log("App resumed to FOREGROUND (Unpaused).");
        }
    }

    private void TriggerAutoSave()
    {
        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.SaveGame();
        }
    }
}