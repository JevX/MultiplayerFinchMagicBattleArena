using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class CalibrationAngelCorrector : MonoBehaviour
{
    private TutorialStep step;

    void Start()
    {
        step = gameObject.GetComponent<TutorialStep>();
    }

    void Update()
    {
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            step.MaxRotationDelta = 30f;
        }
        else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            step.MaxRotationDelta = 10;
        }
    }
}
