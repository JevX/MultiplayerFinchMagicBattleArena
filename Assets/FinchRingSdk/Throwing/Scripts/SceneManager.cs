using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;
using UnityEngine.XR;

public class SceneManager : MonoBehaviour
{
    public GameObject FindSurface;
    public TutorFindSurface SurfaseFindHint;
    public GameObject ThrowingHint;
    public GameObject TapHint;
    public ARTapPosition TapPos;

    bool podiumHere = false;

    private void Start()
    {
        CenterFigureAnimator.podiumHere += podiumInScene;
    }

    // Update is called once per frame
    void Update()
    {
        ThrowingHint.SetActive(SurfaseFindHint.IsStoped && !FinchCalibration.IsCalibrating && podiumHere);
        TapHint.SetActive(SurfaseFindHint.IsStoped && !FinchCalibration.IsCalibrating && !podiumHere);
        FindSurface.SetActive(!FinchCalibration.IsCalibrating);
        TapPos.enabled = !FinchCalibration.IsCalibrating;
    }

    void podiumInScene()
    {
        podiumHere = true;
    }

    public static object LoadSceneAsync(string sceneLoading)
    {
        throw new System.NotImplementedException();
    }
}
