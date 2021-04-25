using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class ArrowHint : MonoBehaviour
{
    public GameObject ArrowLeft;
    public GameObject ArrowRight;
    public GameObject Podium;

    void Update()
    {
        if (Podium == null)
        {
            ArrowLeft.SetActive(false);
            ArrowRight.SetActive(false);
            Podium = GameObject.Find("Podium AR(Clone)");
            return;
        }

        Vector3 targetDir = Podium.transform.position - transform.position;
        Vector3 forward = transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        if (angle < -25.0F)
        {
            ArrowRight.SetActive(!FinchCalibration.IsCalibrating);
            ArrowLeft.SetActive(false);
        }
        else if (angle > 25.0F)
        {


            ArrowLeft.SetActive(!FinchCalibration.IsCalibrating);
            ArrowRight.SetActive(false);

        }
        else
        {
            ArrowLeft.SetActive(false);
            ArrowRight.SetActive(false);
        }
    }
}
