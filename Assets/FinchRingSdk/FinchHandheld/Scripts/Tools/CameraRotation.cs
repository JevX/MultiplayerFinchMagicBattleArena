using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraRotation : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            GetComponent<Camera>().fieldOfView = 40f;
        }
        else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            GetComponent<Camera>().fieldOfView = 75f;
        }
    }
}
