using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class AppButtonDeleteAll : MonoBehaviour
{
    void Update()
    {
        if (FinchCalibration.IsCalibrating)
        {
            DrawLine.DeleteAll();
        }
    }

    public void DeleteAll()
    {
        DrawLine.DeleteAll();
    }
}
