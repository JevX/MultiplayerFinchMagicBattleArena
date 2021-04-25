using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    public float rotationSpeed = 5f;

    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.fixedDeltaTime, Space.Self);
    }
}
