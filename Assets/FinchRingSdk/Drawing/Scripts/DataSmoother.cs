using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataSmoother
{
    public Vector3 SmoothData { get; private set; }
    public Vector3 RawData { get { return value[0]; } }

    private float[] weights;
    private Vector3[] value;

    public DataSmoother(float[] weightKoeff)
    {
        float weightsSum = 0;

        foreach (var i in weightKoeff)
        {
            weightsSum += Mathf.Max(i, 0);
        }

        weightsSum = Mathf.Max(weightsSum, 0.001f);
        weights = new float[weightKoeff.Length];

        for (int i = 0; i < weightKoeff.Length; i++)
        {
            weights[i] = weightKoeff[i] / weightsSum;
        }

        value = new Vector3[weightKoeff.Length];
    }

    public void ResetData(Vector3 data)
    {
        for (int i = weights.Length - 1; i >= 0; i--)
        {
            value[i] = data;
        }

        SmoothData = data;
    }

    public void UpdateData(Vector3 data)
    {
        for (int i = weights.Length - 1; i > 0; i--)
        {
            value[i] = value[i - 1];
        }

        value[0] = data;

        Vector3 sumValues = Vector3.zero;

        for (int i = 0; i < weights.Length; i++)
        {
            sumValues += value[i] * weights[i];
        }

        SmoothData = sumValues;
    }
}
