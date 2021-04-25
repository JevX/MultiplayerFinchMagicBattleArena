using UnityEngine;
using Finch;
using System.Collections;
using MoreMountains.Tools;

public class TutorialButtonClickCheck : MMSingleton<TutorialButtonClickCheck>
{
    public int stepNumber;
    //public GameObject panelComplete;
    //public GameObject imageOk;

    //Coroutine mainCoroutine;

    private void Update()
    {
        if ((stepNumber != -1) && (FinchController.GetPressDown(Chirality.Any, RingElement.HomeButton) || HIDController.Instance.OnPressDownTimes(2)))
        {
            Debug.Log("Finger pressed down CLICK");

            //if ((stepNumber == 3) && (ARTapToPlaceObject.Instance.isCanStand))
            //    TutorStep3Complete();
            //else
            //TutorStepComplete();
            CompleteStep();
            //StartCoroutine(CompleteStep());
            //if (stepNumber + 1 <= 5)
            //{
            //stepNumber += 1;

            //switch (stepNumber)
            //{
            //    case 1:
            //        TutorialSteps.Instance.Step1_Done();
            //        break;
            //    case 2:
            //        TutorialSteps.Instance.Step2_Done();
            //        break;
            //    case 3:
            //        ARTapToPlaceObject.Instance.ButtonClickToStand();
            //        //TutorialSteps.Instance.Step3_Done();
            //        break;
            //    case 4:
            //        ARTapToPlaceObject.Instance.ButtonClickToStart();
            //        //TutorialSteps.Instance.Step4_Done();
            //        break;
            //    case 5:
            //        TutorialSteps.Instance.Step5_Done();
            //        break;
            //    case 6:
            //        TutorialSteps.Instance.Step6_Done();
            //        break;
            //    default:
            //        break;
            //}
            //}
        }
    }

    //public void TutorStepComplete()
    //{
    //    StartCoroutine(CompleteStep());
    //}

    //public void TutorStep3Complete()
    //{
    //    StartCoroutine(CompleteStep3());
    //}

    public void CompleteStep()
    {
        //panelComplete.SetActive(true);
        //imageOk.SetActive(true);
        ////if (stepNumber != 3)
        //yield return new WaitForSeconds(2f);
        //imageOk.SetActive(false);
        //panelComplete.SetActive(false);

        switch (stepNumber)
        {
            case 1:
                //if (mainCoroutine != null)
                //    StopCoroutine(mainCoroutine);
                //mainCoroutine = StartCoroutine(ShowCompleteImage());

                //StartCoroutine(ShowCompleteImage());                
                TutorialSteps.Instance.Step1_Done(); // идет по другому пути (-1)
                //StopCoroutine(ShowCompleteImage());
                break;
            case 2:
                //StartCoroutine(ShowCompleteImage());
                //if (mainCoroutine != null)
                //    StopCoroutine(mainCoroutine);
                //mainCoroutine = StartCoroutine(ShowCompleteImage());

                TutorialSteps.Instance.Step2_Done();
                //StopCoroutine(ShowCompleteImage());
                break;
            case 3:
                if (ARTapToPlaceObject.Instance.isCanStand)
                {
                    //StartCoroutine(ShowCompleteImage());
                    //if (mainCoroutine != null)
                    //    StopCoroutine(mainCoroutine);
                    //mainCoroutine = StartCoroutine(ShowCompleteImage());

                    ARTapToPlaceObject.Instance.isCanMove = false;
                    ARTapToPlaceObject.Instance.ButtonClickToStand();
                    //StopCoroutine(ShowCompleteImage());
                }
                //TutorialSteps.Instance.Step3_Done();
                break;
            case 4:
                //StartCoroutine(ShowCompleteImage());
                //if (mainCoroutine != null)
                //    StopCoroutine(mainCoroutine);
                //mainCoroutine = StartCoroutine(ShowCompleteImage());

                ARTapToPlaceObject.Instance.ButtonClickToStart();
                //StopCoroutine(ShowCompleteImage());
                //TutorialSteps.Instance.Step4_Done();
                break;
            case 5:
                //StartCoroutine(ShowCompleteImage());
                //if (mainCoroutine != null)
                //    StopCoroutine(mainCoroutine);
                //mainCoroutine = StartCoroutine(ShowCompleteImage());

                TutorialSteps.Instance.Step5_Done(); // идет по другому пути (-1)
                //StopCoroutine(ShowCompleteImage());
                break;
            case 6:
                //StartCoroutine(ShowCompleteImage());
                //if (mainCoroutine != null)
                //    StopCoroutine(mainCoroutine);
                //mainCoroutine = StartCoroutine(ShowCompleteImage());

                TutorialStep6_InfoTarget.Instance.HideStep();
                TutorialSteps.Instance.Step6_Done(); // идет по другому пути (-1)
                //StopCoroutine(ShowCompleteImage());
                //TutorialButtonClickCheck.Instance.stepNumber = 6;
                //TutorialButtonClickCheck.Instance.TutorStepComplete();
                //TutorialStep6_InfoTarget.Instance.HideStep();
                break;
            default:
                break;
        }
        //if (stepNumber == 3)
        //yield return new WaitForSeconds(2f);
        //imageOk.SetActive(false);
        //panelComplete.SetActive(false);
    }

    //private IEnumerator ShowCompleteImage()
    //{
    //    panelComplete.SetActive(true);
    //    imageOk.SetActive(true);
    //    //if (stepNumber != 3)
    //    yield return new WaitForSeconds(2f);
    //    imageOk.SetActive(false);
    //    panelComplete.SetActive(false);
    //    //StopCoroutine(ShowCompleteImage());
    //}

    //public IEnumerator CompleteStep3()
    //{
    //    //if (ARTapToPlaceObject.Instance.ButtonClickToStand())
    //    //{
    //        panelComplete.SetActive(true);
    //        imageOk.SetActive(true);
    //        yield return new WaitForSeconds(2f);
    //        imageOk.SetActive(false);
    //        panelComplete.SetActive(false);
    //    //}
      
    //}

}
