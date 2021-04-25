using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CardboardDisabler : MonoBehaviour
{
    bool inited = false;

    void Awake()
    {
        XRSettings.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Init())
        {
            Camera.main.GetComponent<Transform>().localRotation = InputTracking.GetLocalRotation(XRNode.CenterEye);

            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                Camera.main.ResetAspect();
            }
            else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                Camera.main.ResetAspect();
            }
        }
    }

    bool Init()
    {
        XRSettings.enabled = false;
        return true;
    }
}
