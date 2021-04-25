using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class PositionPodium_3dof_6dof : MonoBehaviour
{
    public Transform Head;

    public Vector3 podium_position_6dof;
    public Vector3 podium_position_3dof;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (FinchCalibration.IsCalibrating)
        {
            bool b3dof = (Finch.FinchNodeManager.GetUpperArmCount() == 0);

            Vector3 distance = b3dof ? podium_position_3dof : podium_position_6dof;

            //transform.position = b3dof ? podium_position_3dof : podium_position_6dof;
            Vector3 tempVector = Head.position + Head.forward * distance.z;
            transform.position = new Vector3(tempVector.x, distance.y, tempVector.z);
        }
    }
}
