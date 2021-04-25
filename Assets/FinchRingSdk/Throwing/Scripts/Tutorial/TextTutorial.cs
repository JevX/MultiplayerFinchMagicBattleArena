using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;
using UnityEngine.UI;
using System;

[Serializable]
public class TextDescriptor
{
    public Text Text1;
    public Text Text2;
    public Text Text3;

    public Text Text1vert;
    public Text Text2vert;
    public Text Text3vert;

    public Text Text1hor;
    public Text Text2hor;
    public Text Text3hor;

    public float speedAnim = 0.25f;
    public Color color;
}

public class TextTutorial : MonoBehaviour
{
    public TextDescriptor animationDescription = new TextDescriptor();

    private int index = 0;

    public GameObject root;
    public Chirality Chirality;
    public string CurrentText;

    private float alpha1 = 0;
    private float alpha2 = 0;
    private float alpha3 = 0;

    private bool bCalibrationEnd = false;

    public Vector3 DistanceFromHMD;
    public float MaxRotationDelta = 30;
    public float StithnessY = 10f;
    private float angle;
    private FinchCalibration calib;
    public bool bIn_AR_scene = true;

    private ObjectPickup objPickupR;
    private ObjectPickup objPickupL;
    private ConnectionAnyStep calibrateRefPosition;
    private ManagerPiramydRigidbodies managerPiramyd;

    private void Awake()
    {
    }

    private void Start()
    {
        calib = GameObject.Find("FinchCalibration").GetComponent<FinchCalibration>();
        managerPiramyd = GameObject.Find("ManagerPiramydRigidbodies").GetComponent<ManagerPiramydRigidbodies>();

        if (bIn_AR_scene)
            CenterFigureAnimator.podiumHere += CalibrationEnd;
        else
            Finch.FinchCalibration.OnCalibrationEnd += CalibrationEnd;
    }

    void CalibrationEnd()
    {
        bCalibrationEnd = true;
    }

    void Update()
    {
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            animationDescription.Text1.text = animationDescription.Text1hor.text;
            animationDescription.Text2.text = animationDescription.Text2hor.text;
            animationDescription.Text3.text = animationDescription.Text3hor.text;
        }
        else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            animationDescription.Text1.text = animationDescription.Text1vert.text;
            animationDescription.Text2.text = animationDescription.Text2vert.text;
            animationDescription.Text3.text = animationDescription.Text3vert.text;
        }

        UpdatePosition();//update position text for camera

        if (FinchCalibration.IsCalibrating)
        {
            root.gameObject.SetActive(false);
            // CurrentText = "";
            return;
        }
        else if (bCalibrationEnd)
        {
            root.gameObject.SetActive(true);
        }


        if (FinchController.GetPress(Chirality, RingElement.Touch))
        {
            // CurrentText = "Press and hold touchpad button \nto grab the object ";
            index = 1;
            if (FinchController.GetPress(Chirality, RingElement.HomeButton) && managerPiramyd.IsPickUped())
            {
                // CurrentText = "Wave your hand and release \nthe touchpad button \nto throw the object"; 
                index = 2;
            }
        }
        else
        {
            // CurrentText = "Touch and hold the touchpad \nto show the controller ";
            index = 3;
        }

        //////////////////////// Lerp color processing
        alpha1 = MathUtility.Lerp(alpha1, index == 1 ? 1 : 0, Time.deltaTime, animationDescription.speedAnim);
        alpha2 = MathUtility.Lerp(alpha2, index == 2 ? 1 : 0, Time.deltaTime, animationDescription.speedAnim);
        alpha3 = MathUtility.Lerp(alpha3, index == 3 ? 1 : 0, Time.deltaTime, animationDescription.speedAnim);

        Color c1 = animationDescription.color;
        Color c2 = animationDescription.color;
        Color c3 = animationDescription.color;

        c1.a = alpha1;
        c2.a = alpha2;
        c3.a = alpha3;

        animationDescription.Text1.color = c1;
        animationDescription.Text2.color = c2;
        animationDescription.Text3.color = c3;
        ///////////////////////////////////////////////////
    }

    protected void UpdatePosition()
    {
        float delta = calib.CalibrationOptions.Head.eulerAngles.y - angle;

        if (Mathf.Abs(delta) > 180)
        {
            delta = (Mathf.Abs(delta) - 360) * Mathf.Sign(delta);
        }

        if (Mathf.Abs(delta) > MaxRotationDelta)
        {
            float resultAngle = (calib.CalibrationOptions.Head.eulerAngles.y - Mathf.Sign(delta) * MaxRotationDelta);
            delta = resultAngle - angle;

            if (Mathf.Abs(delta) > 180)
            {
                delta = (Mathf.Abs(delta) - 360) * Mathf.Sign(delta);
            }

            angle += delta * (MaxRotationDelta == 0 ? 1 : Mathf.Clamp01(Time.deltaTime * StithnessY));
        }

        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

        Vector3 deltaX = DistanceFromHMD.x * (Vector3.right * cos + Vector3.forward * sin);
        Vector3 deltaY = DistanceFromHMD.y * Vector3.up;
        Vector3 deltaZ = DistanceFromHMD.z * (Vector3.right * sin + Vector3.forward * cos);

        transform.position = calib.CalibrationOptions.Head.position + deltaX + deltaY + deltaZ;
        transform.LookAt(calib.CalibrationOptions.Head.position, Vector3.up);
        transform.eulerAngles = Vector3.up * transform.eulerAngles.y;
    }
}
