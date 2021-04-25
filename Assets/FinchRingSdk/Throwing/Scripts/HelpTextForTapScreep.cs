using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpTextForTapScreep : MonoBehaviour
{

    public GameObject messagePanel;
    private bool bCalib_done = false;
    private bool bPodium_here = false;
    public bool bIn_AR_kit = true;

    // Use this for initialization
    void Start()
    {
        //messagePanel = GameObject.Find("TapScreenHelpMessage"); //find panel and dissable on start
        messagePanel.gameObject.SetActive(false);

        if (bIn_AR_kit)
            TutorFindSurface.searchFinish += calib_done;
        else
            Finch.FinchCalibration.OnCalibrationEnd += calib_done;

        CenterFigureAnimator.podiumHere += podiumInScene;
    }

    void podiumInScene()
    {
        bPodium_here = true;
    }

    void calib_done()
    {
        bCalib_done = true;
    }

    // Update is called once per frame
    void Update()
    {
        //////////////////////////rotate message panel from camera
        if (Mathf.Abs(Camera.main.transform.rotation.z) > 0.8f)
        {
            messagePanel.transform.localRotation = new Quaternion(messagePanel.transform.localRotation.x, messagePanel.transform.localRotation.y, 180, 0);
        }
        else
        {
            messagePanel.transform.localRotation = new Quaternion(messagePanel.transform.localRotation.x, messagePanel.transform.localRotation.y, 0, 0);
        }
        /////////////////////////

        bool target_active = bCalib_done && !bPodium_here;
        if (target_active != messagePanel.gameObject.activeInHierarchy)
            messagePanel.gameObject.SetActive(target_active);
    }
}
