using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class Hider : MonoBehaviour
{
    public GameObject Controller;
    public GameObject Line;
    public ThrowObjectsContainer Objects;

    public Vector3 Three;
    public Vector3 Six;

    private int lastModeWasThreeDof = -1;

    void Awake()
    {
        FinchCalibration.OnCalibrationEnd += Replace;
    }

    void Update()
    {
        Controller.SetActive(FinchController.GetPress(Chirality.Right, RingElement.Touch));
        Line.SetActive(FinchController.GetPress(Chirality.Right, RingElement.Touch) && FinchNodeManager.GetUpperArmCount() == 0);
    }

    void Replace()
    {
        if (FinchNodeManager.GetUpperArmCount() > 0)
        {
            if (lastModeWasThreeDof != 0)
            {
                Replace(Six, newState: 0, startReplace: (lastModeWasThreeDof == -1));
            }
        }
        else
        {
            if (lastModeWasThreeDof != 1)
            {
                Replace(Three, newState: 1, startReplace: (lastModeWasThreeDof == -1));
            }
        }
    }

    void Replace(Vector3 position, int newState, bool startReplace)
    {
        if (!startReplace)
        {
            Objects.Comeback();
        }
        Objects.transform.position = position;
        Objects.Remember();
        lastModeWasThreeDof = newState;
    }
}
