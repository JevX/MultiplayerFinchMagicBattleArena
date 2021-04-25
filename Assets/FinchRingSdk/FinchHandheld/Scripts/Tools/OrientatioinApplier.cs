using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientatioinApplier : OrientationBase
{
    public enum State
    {
        Position,
        Scale
    }

    public State DynamicState;

    public Vector3 PortraitOrientationData;
    public Vector3 LandscapeOrientationData;

    private void OnEnable()
    {
        Update();
    }

    void Update () {
        Vector3 data = GetScreenOrientation() == ScreenOrientation.Landscape ? LandscapeOrientationData : PortraitOrientationData;

        switch (DynamicState)
        {
            case State.Position:
                transform.localPosition = data;
                break;

            case State.Scale:
                transform.localScale = data;
                break;
        }
	}
}
