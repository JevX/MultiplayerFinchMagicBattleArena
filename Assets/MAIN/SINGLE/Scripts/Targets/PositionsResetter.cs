using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsResetter : MonoBehaviour
{
    public Transform target;
    Transform[] childTransforms;
    Vector3[] startPositions;
    Quaternion[] startRotations;

    private void Awake()
    {
        childTransforms = target.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < childTransforms.Length; i++)
        {
            startPositions[i] = childTransforms[i].localPosition;
            startRotations[i] = childTransforms[i].localRotation;
        }
    }

    private void OnEnable()
    {
        ReturnToStartPositions();
        target.gameObject.SetActive(false);
    }

    public void ReturnToStartPositions()
    {
        for (int i = 0; i < childTransforms.Length; i++)
        {
            childTransforms[i].localPosition = startPositions[i];
            childTransforms[i].localRotation = startRotations[i];
        }
    }
}
