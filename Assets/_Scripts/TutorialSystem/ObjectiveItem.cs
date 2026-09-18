using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ObjectiveItem : MonoBehaviour
{
    public TutorialStep step;

    public void CheckTutorial()
    {
        TutorialSystem.instance.DoTutorial(step);
    }
}