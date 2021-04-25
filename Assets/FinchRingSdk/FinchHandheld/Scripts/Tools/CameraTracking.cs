using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
#if !UNITY_EDITOR
    private float ZCompensator = 45;
    private float XCompensator = 90;

    private void Awake()
    {
        Input.gyro.enabled = true;
    }

    private void Update()
    {
        Quaternion gyro = Input.gyro.attitude;
        Quaternion unity = new Quaternion(gyro.x, gyro.y, -gyro.z, -gyro.w);

        Quaternion result = Quaternion.AngleAxis(XCompensator, Vector3.right) * (Quaternion.AngleAxis(ZCompensator, Vector3.forward) * unity);
        transform.rotation = result;
    }
#endif
}
