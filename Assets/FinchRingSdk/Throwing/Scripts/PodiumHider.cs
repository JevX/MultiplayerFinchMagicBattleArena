using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class PodiumHider : MonoBehaviour
{
    public MeshRenderer[] MeshesToHide;
    public GameObject[] ObjectsToHide;

    void Update()
    {
        foreach (MeshRenderer mesh in MeshesToHide)
        {
            mesh.enabled = !FinchCalibration.IsCalibrating;
        }

        foreach (GameObject obj in ObjectsToHide)
        {
            obj.SetActive(!FinchCalibration.IsCalibrating);
        }
    }
}
