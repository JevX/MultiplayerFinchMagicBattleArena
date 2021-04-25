using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDestroy : MonoBehaviour
{
    private void OnDestroy()
    {
        Debug.Log("I am destroyed");
    }
}
