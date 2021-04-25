using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class DrawingWallPosFixer : MonoBehaviour
{
    public Transform Head;
    public Transform Wall;
    public Vector3 Distance;

    // Update is called once per frame
    void Update()
    {
        if (FinchCalibration.IsCalibrating)
        {
            Vector3 tempVector = Head.position + Head.forward * Distance.z;
            Wall.position = new Vector3(tempVector.x, Distance.y, tempVector.z);
            Wall.LookAt(Head.position);
        }
    }
}
