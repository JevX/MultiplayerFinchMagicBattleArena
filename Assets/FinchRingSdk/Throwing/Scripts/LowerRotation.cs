using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;
using UnityEngine.UI;
using System;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.iOS;
#endif

public class LowerRotation : MonoBehaviour
{
    public float distanceVertical;
    public float distanceHorisontal;

    private Vector3 position;

    void Start()
    {
        position = transform.localPosition;
    }

    private void OnEnable()
    {
        position = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (Device.generation >= DeviceGeneration.iPhoneX)

            if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                transform.localPosition = new Vector3(position.x, position.y - distanceHorisontal, position.z);
            }
            else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                transform.localPosition = new Vector3(position.x, position.y - distanceVertical, position.z);
            }
#endif
    }
}
