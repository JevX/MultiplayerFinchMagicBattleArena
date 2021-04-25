using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class CalibrationNotificationManager : MonoBehaviour
{
    public GameObject Notificator;
    public GameObject Header;
    public GameObject BG;

    void Update()
    {
        if (Notificator)
            Notificator.SetActive(FinchCalibration.IsCalibrating);

        if (Header)
            Header.SetActive(FinchCalibration.IsCalibrating);

        if (BG)
            BG.SetActive(FinchCalibration.IsCalibrating);
    }
}
