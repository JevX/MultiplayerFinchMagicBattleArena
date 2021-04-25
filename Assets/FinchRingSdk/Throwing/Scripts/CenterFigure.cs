using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterFigure : MonoBehaviour
{


    //public float speed_user_rotation = 10;
    //[Header("Scaling")]
    //public GameObject[] ScaleReciviers;
    //public float scale = 1;
    //public float frenqucy = 8;
    //private Vector3 startScale;
    //private float last_scale = 0;
    //public float maxScale = 0.9f;
    //public float minScale = 0.1f;
    //public float duration;
    //public float duration2;
    //public float scale_mod;

    [Header("Swivel")]
    public float SwileConeX = 3;
    public float SwileConeY = 1;

    public float SwivelPeriodX = 20;
    public float SwivelPeriodY = 14;

    float CurrentSwivelX = 0;
    float CurrentSwivelY = 0;

    private float user_rotation = 0;
    private float user_rotation_curent = 0;

    [Header("Bobbing")]
    public float BobbingPeriod1 = 52;
    public float BobbingAmplitude1 = 23;
    public float BobbingPeriod2 = 29;
    public float BobbingAmplitude2 = 9;
    public float BobbingPeriod3 = 11;
    public float BobbingAmplitude3 = 5;

    float BobbingCycle1 = 0;
    float BobbingCycle2 = 0;
    float BobbingCycle3 = 0;




    Vector3 originPosition;


    void Start()
    {
        originPosition = transform.localPosition;
       // startScale = transform.localScale;
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
            Norm = (Mathf.Abs(NewX.z) < (1.0f - 0.0001f)) ? Vector3.up : Vector3.forward;
        }

        Vector3 NewZ = Vector3.Cross(NewX, Norm).normalized;
        Vector3 NewY = Vector3.Cross(NewZ, NewX).normalized;

        return new Matrix4x4((Vector4)NewX, (Vector4)NewY, (Vector4)NewZ, new Vector4(0, 0, 0, 1));
    }

    void Update()
    {


        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    print("right");
        //    user_rotation += -200 * Time.deltaTime;
        //    //transform.Rotate(Vector3.up, -200 * Time.deltaTime );
        //}
        //else if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    print("left");
        //    user_rotation += 200 * Time.deltaTime;
        //    // transform.Rotate(Vector3.up, 200 * Time.deltaTime );
        //}
        //user_rotation_curent = Lerp(user_rotation_curent, user_rotation, Time.deltaTime, speed_user_rotation);
        Swiveling();
       // Scale();
        Floating();
    }
    float Lerp(float Current, float Target, float DeltaTime, float InterpSpeed)
    {
        if (InterpSpeed <= 0.0f) return Target;
        float Alpha = Mathf.Clamp(DeltaTime * InterpSpeed, 0.0f, 1.0f);
        return Target * Alpha + Current * (1.0f - Alpha);
    }
    void Scale()
    {
        //if (scale >= maxScale)
        //{
        //    scale = maxScale;
        //    if (last_scale != scale)
        //    {
        //        StartCoroutine(Shake(true));
        //        last_scale = scale;

        //    }

        //}
        //else if (scale <= minScale)
        //{
        //    scale = minScale;
        //    if (last_scale != scale)
        //    {
        //        StartCoroutine(Shake(false));
        //        last_scale = scale;

        //    }
        //}
        //else
        //{
        //    Vector3 _scale = new Vector3(startScale.x + scale, startScale.y + scale, startScale.z + scale); ;
        //    for (int i = 0; i < ScaleReciviers.Length; ++i)
        //    {
        //        ScaleReciviers[i].transform.localScale = _scale;
        //    }
        //    last_scale = 0;
        //}
    }

    private void Floating()
    {
        BobbingCycle1 = (Time.deltaTime * 360 / BobbingPeriod1 + BobbingCycle1) % 360;
        BobbingCycle2 = (Time.deltaTime * 360 / BobbingPeriod2 + BobbingCycle2) % 360;
        BobbingCycle3 = (Time.deltaTime * 360 / BobbingPeriod3 + BobbingCycle3) % 360;

        float newY = (Mathf.Sin(BobbingCycle1 * Mathf.Deg2Rad) * BobbingAmplitude1) +
            (Mathf.Sin(BobbingCycle2 * Mathf.Deg2Rad) * BobbingAmplitude2) +
            (Mathf.Sin(BobbingCycle3 * Mathf.Deg2Rad) * BobbingAmplitude3);
        Vector3 makeVector = new Vector3(0, newY, 0);
        transform.localPosition = originPosition + makeVector;
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

        transform.rotation = M.rotation * Quaternion.Euler(0, 90 + user_rotation_curent, 0);
    }

    //IEnumerator Shake(bool side)
    //{
    //    print("Shake");
    //    Vector3 originalPos = ScaleReciviers[0].transform.localScale;
    //    float elapsed = 0.0f;
    //    while (elapsed < duration)
    //    {
    //        float x = Mathf.Clamp01(elapsed / duration);
    //        float s = Mathf.Sin(Mathf.Pow(x,2) * Mathf.PI * frenqucy) * Mathf.Pow(1 - x,2); //sin((x^2)*3.1415*12)*((1-x)^2)
    //        if(!side)
    //            s *= -1;
    //        s *= scale_mod;
    //        s += 1;
    //        for (int i = 0; i < ScaleReciviers.Length; ++i)
    //        {
    //            ScaleReciviers[i].transform.localScale = originalPos * s;
    //        }
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    for (int i = 0; i < ScaleReciviers.Length; ++i)
    //    {
    //        ScaleReciviers[i].transform.localScale = originalPos;
    //    }
    //}

    //IEnumerator Shake()
    //{
    //    print("Shake");
    //    Vector3 originalPos = ScaleReciviers[0].transform.localScale;
    //    float elapsed = 0.0f;
    //    int last = -1;
    //    while (elapsed < duration || last == 1)
    //    {
    //        float attenuation = Mathf.Clamp01(elapsed / duration);
    //        float x = Random.Range(originalPos.x, originalPos.x + scale_mod * attenuation);
    //        Vector3[] array = new Vector3[ScaleReciviers.Length];
    //        for (int i = 0; i < ScaleReciviers.Length; ++i)
    //        {
    //            array[i] = ScaleReciviers[i].transform.localScale;
    //        }
    //        Vector3 _scale = new Vector3(x, x, x);
    //        if (last == 1)
    //            _scale = originalPos;
    //        float elapsed2 = 0.0f;
    //        while (elapsed2 < duration2)
    //        {
    //            float alpha = elapsed2 / duration2;
    //            for (int i = 0; i < ScaleReciviers.Length; ++i)
    //            {
    //                ScaleReciviers[i].transform.localScale = Vector3.Lerp(array[i], _scale, alpha);
    //            }
    //            elapsed2 += Time.deltaTime;
    //            yield return null;
    //        }
    //        elapsed += elapsed2 + Time.deltaTime;

    //        if (last == 1)
    //            break;
    //        if (elapsed >= duration && last == -1)// 
    //            last = 1;

    //        yield return null;
    //    }

    //    for (int i = 0; i < ScaleReciviers.Length; ++i)
    //    {
    //        ScaleReciviers[i].transform.localScale = originalPos;
    //    }
    //}
}
