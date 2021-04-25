using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class DrawingSceneManagement : MonoBehaviour
{
    public GameObject DrawingObject;
    public Drawer DrawerLeft;
    public Drawer DrawerRight;
    public GameObject DrawingWall;
    public GameObject DrawTutorial;
    public bool IsAR;
    public GameObject SurfaceHint;
    public DrawingTutorFindSurface FindSurfaceHint;
    public ARPlaneManager PlaneManager;

    private bool iSTouched = false;
    private bool alreadyTouched = false;
    private bool notEndYet = false;

    void Update ()
    {
        if (IsAR)
            PlaneManager.enabled = !FinchCalibration.IsCalibrating && FinchNodeManager.GetUpperArmCount() == 0;

        FinchCalibration.OnCalibrationEnd += CheckHint;

        if (FinchCalibration.IsCalibrating)
        {
            DrawerLeft.RecalibrationEnd = false;
            DrawerRight.RecalibrationEnd = false;
        }

        DrawingObject.SetActive(!FinchCalibration.IsCalibrating);

        if (FinchController.GetPressDown(Chirality.Any, RingElement.HomeButton) && !FinchCalibration.IsCalibrating)
        {
            iSTouched = true;
        }

        if (alreadyTouched && FinchController.GetPressUp(Chirality.Any, RingElement.HomeButton))
            alreadyTouched = false;

        DrawTutorial.SetActive(IsAR && FindSurfaceHint.IsStoped && !(iSTouched && !alreadyTouched) && !FinchCalibration.IsCalibrating);

        if (IsAR)
        {
            SurfaceHint.SetActive(!DrawTutorial.activeSelf && !FinchCalibration.IsCalibrating);
        }
            
        if (DrawingWall)
            DrawingWall.SetActive(!FinchCalibration.IsCalibrating && FinchNodeManager.GetUpperArmCount() == 0);
    }

    void CheckHint()
    {
    	if (notEndYet)
    		return;

        if ((FinchController.GetPress(Chirality.Any, RingElement.Touch) || FinchController.GetPress(Chirality.Any, RingElement.HomeButton)))
            alreadyTouched = true;

        if (!IsAR)
            DrawTutorial.SetActive(true);

        FinchCalibration.OnCalibrationEnd -= CheckHint;
        notEndYet = true;
    }
}
