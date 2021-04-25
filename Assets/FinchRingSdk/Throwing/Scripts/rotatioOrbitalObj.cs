using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatioOrbitalObj : MonoBehaviour
{
    // Start is called before the first frame update
    private float SwileConeX;
    private float SwileConeY;
    
    float SwivelPeriodX = 4;
    float SwivelPeriodY = 6;

    float CurrentSwivelX = 0;
    float CurrentSwivelY = 0;
    void Start()
    {
       // transform.rotation = Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180));
        SwivelPeriodX = Random.Range(4,9);
        SwivelPeriodY = Random.Range(4, 9);
        SwileConeX = Random.Range(10, 30);
        SwileConeY = Random.Range(10, 30);
    }

    // Update is called once per frame
    void Update()
    {
        Swiveling();
    }
    private void Swiveling()
    {
        Vector3 SwivelAxisX;
        Vector3 SwivelAxisY;

        //rotate vector around axis
        SwivelAxisX = Quaternion.Euler(0, SwileConeX, 0) * Vector3.forward;
        SwivelAxisY = Quaternion.Euler(0, SwileConeY, 0) * Vector3.right;

        CurrentSwivelX = ((Time.deltaTime * 360 / SwivelPeriodX) + CurrentSwivelX) % 360;
        CurrentSwivelY = ((Time.deltaTime * 360 / SwivelPeriodY) + CurrentSwivelY) % 360;

        Vector3 X = Quaternion.Euler(0, 0, CurrentSwivelX) * SwivelAxisX;
        Vector3 Y = Quaternion.Euler(CurrentSwivelY, 0, 0) * SwivelAxisY;
        
        Matrix4x4 M = MakeFromXY(X, Y);

        transform.rotation = M.rotation*Quaternion.Euler(0,90,0);
    }
    Matrix4x4 MakeFromXY(Vector3 XAxis, Vector3 YAxis)
    {
        Vector3 NewX = XAxis.normalized;
        Vector3 Norm = YAxis.normalized;

        float dot = Vector3.Dot(NewX, Norm);
        dot = Mathf.Abs(dot);

        // if they're almost same, we need to find arbitrary vector
        if (dot <= 1.0f)
        {
            // make sure we don't ever pick the same as NewX
            Norm = (Mathf.Abs(NewX.z) < (1.0f - 0.0001f) ) ? Vector3.up : Vector3.forward;
        }

        Vector3 NewZ = Vector3.Cross(NewX, Norm).normalized;
        Vector3 NewY = Vector3.Cross(NewZ, NewX).normalized;

        return new Matrix4x4((Vector4)NewX, (Vector4)NewY, (Vector4)NewZ, new Vector4(0,0,0,1));
    }
}
