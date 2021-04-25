using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMatScript : MonoBehaviour
{
    public Material mat;


    private void FixedUpdate()
    {
        mat.mainTextureOffset += Vector2.right * Time.fixedDeltaTime;
    }
}
