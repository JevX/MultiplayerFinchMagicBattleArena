using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testActivateDot : MonoBehaviour
{
    private void Awake()
    {
       
    }

    private void Start()
    {
        ProgressionManager.Instance.StartNewStage();
    }
}
