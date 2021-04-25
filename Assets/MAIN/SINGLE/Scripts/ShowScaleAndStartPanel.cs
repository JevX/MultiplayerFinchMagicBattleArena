using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowScaleAndStartPanel : MonoBehaviour
{
    public GameObject panelScaleAndStart;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Finch.FinchCalibration.IsCalibrating /*&& ARTapToPlaceObject.Instance.isButtonStartClick*/) panelScaleAndStart.SetActive(true);
    }
}
