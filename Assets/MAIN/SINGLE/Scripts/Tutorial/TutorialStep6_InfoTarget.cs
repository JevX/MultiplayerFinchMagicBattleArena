using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using Finch;

public class TutorialStep6_InfoTarget : MMSingleton<TutorialStep6_InfoTarget>//, MMEventListener<MMGameEvent>
{
    //public GameObject panelScoreManager;
    public GameObject panelTargetInfo;    //step 5 - target info
    public GameObject panelMagicBookInfo; //step 6 - book info 

    public List<GameObject> listOfMagicScrolls;
    //public Image imageScroll;
    private int currentCount = 0;
    private int prevCount = 0;
    private int nextCount = 0;

    //public Animator animator;        

    private bool updateTimer;

    private bool isStep = false;
    public  bool isStepEnd = false;

    public float waitTime = 10f;
    private float totalTime;

    public GameObject objectTutorialSteps;

    NodeType Node;

    private void Start()
    {
        currentCount = 0;
        nextCount = 1;
        prevCount = listOfMagicScrolls.Count - 1;
        isStep = false;

        for (var i = 0; i < listOfMagicScrolls.Count; i++)
            listOfMagicScrolls[i].SetActive(false);

        listOfMagicScrolls[currentCount].SetActive(true);
    }

    private void HideAllPanels()
    {
        //panelScoreManager.SetActive(false);
        panelTargetInfo.SetActive(false);
        panelMagicBookInfo.SetActive(false);
    }

    private void ToNextPage()
    {
        //if (currentCount + 1 > listOfMagicScrolls.Count)
        //{
        //    //MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step6_done);
        //    //HideStep();
        //    TutorialSteps.Instance.Step6_Done();
        //    panelMagicBookInfo.SetActive(false);
        //    isStepEnd = true;
            
        //}
        //else currentCount++;

        //for (var i = 0; i < listOfMagicScrolls.Count; i++)
        //    listOfMagicScrolls[i].SetActive(false);

        //listOfMagicScrolls[currentCount].SetActive(true);
        //-------------------------------
        currentCount += 1;

        if (currentCount < listOfMagicScrolls.Count)
        {
            for (var i = 0; i < listOfMagicScrolls.Count; i++)
                listOfMagicScrolls[i].SetActive(false);

            listOfMagicScrolls[currentCount].SetActive(true);
        }
        else
        {
            panelMagicBookInfo.SetActive(false);
            //MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step5_done);

            //TutorialButtonClickCheck.Instance.stepNumber = 6;
            //TutorialButtonClickCheck.Instance.TutorStepComplete();


            //TutorialSteps.Instance.Step6_Done(); //если что вернуть
            isStepEnd = true;
            //HideStep();
            
        }
    }

    //private void ToPrevPage()
    //{
    //    if (currentCount - 1 < 0) isStep = true;
    //    else currentCount--;

    //    for (var i = 0; i < listOfMagicScrolls.Count; i++)
    //        listOfMagicScrolls[i].SetActive(false);

    //    listOfMagicScrolls[currentCount].SetActive(true);
    //}

    private void Update()
    {
        //if (updateTimer)
        //{
        //    waitTime -= Time.fixedDeltaTime;
        //    if (waitTime <= 0)
        //    {
        //        //HideStep();
        //        ToNextPage();

        //        if (isStep)
        //        {
        //            MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step5_done);
        //            HideStep();
        //        }

        //        //BoolStepVarOff();
        //    }
        //    //timerImage.fillAmount = waitTime / totalTime;
        //    //TimerText.text = waitTime.ToString("##");

        //}
        //if (TutorialSteps.Instance.isStep5_Complete && !TutorialSteps.Instance.isStep6_Complete && isStepEnd == false)
        //{
        //    if (Finch.FinchInput.GetTouchpadEvent(Node, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeRight))
        //    {
        //        ToNextPage();
        //    }
        //}
    }

    public void ShowStep6()
    {
        objectTutorialSteps.SetActive(true);
        Debug.Log("Step6_1");
        //HideAllPanels();

        //objectTutorialSteps.transform.localScale = new Vector3(0.125f, 0.125f, 1f);

        panelMagicBookInfo.SetActive(true);
        Debug.Log("panelMagicBookInfo.SetActive = "+ panelMagicBookInfo.activeSelf);
        Debug.Log("Step6_2");
        listOfMagicScrolls[0].SetActive(true);
        Debug.Log("Step6_3");
        objectTutorialSteps.transform.position = new Vector3(PlacementController.Instance.transform.localPosition.x, Camera.main.transform.position.y, PlacementController.Instance.transform.localPosition.z);

        objectTutorialSteps.transform.LookAt(Camera.main.transform);
        objectTutorialSteps.transform.localRotation *= Quaternion.Euler(0, 180, 0);


        Debug.Log("Animation Step 6");
        MagicBookStartAnimation.Instance.BookStartAnimation();
        //StageText.text = stage.ToString();
        //TimeText.text = time.ToString();
        //TargetsText.text = targets.ToString();
        //isStep = true;


        //animator.Play("OpenScore");

        //waitTime = showTimer;
        //totalTime = showTimer;
        //updateTimer = true;
    }

    //private void ShowStep6()
    //{
    //    transform.position = new Vector3(PlacementController.Instance.transform.localPosition.x, Camera.main.transform.position.y, PlacementController.Instance.transform.localPosition.z);

    //    transform.LookAt(Camera.main.transform);
    //    transform.localRotation *= Quaternion.Euler(0, 180, 0);

    //    //StageText.text = stage.ToString();
    //    //TimeText.text = time.ToString();
    //    //TargetsText.text = targets.ToString();
    //    isStep = true;

    //    panelMagicBookInfo.SetActive(true);
    //    animator.Play("OpenScore");

    //    //waitTime = showTimer;
    //    //totalTime = showTimer;
    //    updateTimer = true;
    //}

    public void HideStep()
    {
        panelMagicBookInfo.SetActive(false);
        TutorialSteps.Instance.Step6_Done();
        //animator.Play("CloseScore");
        //HideAllPanels();
        //objectTutorialSteps.SetActive(false);
        //updateTimer = false;
    }

    //public void OnMMEvent(MMGameEvent gameEvent)
    //{
    //    switch (gameEvent.EventName)
    //    {
    //        case "GameStarted":
    //            //ShowStep5();
    //            break;

    //        default:
    //            break;
    //    }
    //}

    //private void OnEnable()
    //{
    //    this.MMEventStartListening<MMGameEvent>();
    //}

    //private void OnDisable()
    //{
    //    this.MMEventStopListening<MMGameEvent>();     
    //}
}
