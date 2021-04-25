using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrientationBase : MonoBehaviour
{
    public bool UseInPortraitMode = true;
    public bool UseInLandscapeMode;

    protected bool IsAvailableOrientation
    {
        get
        {
            ScreenOrientation orientation = GetScreenOrientation();
            return orientation == ScreenOrientation.Landscape && UseInLandscapeMode || orientation == ScreenOrientation.Portrait && UseInPortraitMode;
        }
    }

    protected ScreenOrientation GetScreenOrientation()
    {
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer
            || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return ScreenOrientation.Landscape;
        }
        else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            return ScreenOrientation.Portrait;
        }

        bool runtimeLandScape = Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight;
        return runtimeLandScape ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;
    }
   
}
