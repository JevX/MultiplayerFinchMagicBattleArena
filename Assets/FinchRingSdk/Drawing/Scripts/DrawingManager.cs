using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    public static Transform DrawingBase;
    public static bool IsDrawing = true;

    private void Start()
    {
        DrawingBase = transform;
    }
}
