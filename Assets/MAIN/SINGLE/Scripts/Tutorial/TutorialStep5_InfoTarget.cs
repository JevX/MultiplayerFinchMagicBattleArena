using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using Finch;
using System.Collections;

public class TutorialStep5_InfoTarget : MMSingleton<TutorialStep5_InfoTarget>, MMEventListener<MMGameEvent>
{

    public Animator TutorialAnimation;
    //public GameObject panelScoreManager;
    public GameObject panelTargetInfo;    //step 5 - target info
    public GameObject panelMagicBookInfo; //step 6 - book info 

    public List<GameObject> listOfMagicScrolls = new List<GameObject>();
    //public Image imageScroll;
    private int currentCount = 0;
    private int prevCount = 0;
    private int nextCount = 0;

    //public Animator animator;        

    private bool updateTimer;

    private bool isStep = false;
    public bool isStepEnd = false;

    public float waitTime = 10f;
    private float totalTime;  

    [SerializeField] private GameObject objectTutorialSteps;    

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
        Debug.Log("Hided");
        //panelScoreManager.SetActive(false);
        panelTargetInfo.SetActive(false);
        panelMagicBookInfo.SetActive(false);
    }

    //private void ToNextPage()
    //{
    //    //if (currentCount + 1 > listOfMagicScrolls.Count)
    //    //{
    //    //    panelTargetInfo.SetActive(false);
    //    //    //MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step5_done);
    //    //    TutorialSteps.Instance.Step5_Done();
    //    //    isStepEnd = true;
    //    //    //HideStep();
    //    //    TutorialStep6_InfoTarget.Instance.ShowStep6();
                       
    //    //}
    //    //else currentCount++;

    //    //for (var i = 0; i < listOfMagicScrolls.Count; i++)
    //    //    listOfMagicScrolls[i].SetActive(false);

    //    //listOfMagicScrolls[currentCount].SetActive(true);

    //    //--------------

    //    currentCount += 1;

    //    if (currentCount < listOfMagicScrolls.Count)
    //    {
    //        for (var i = 0; i < listOfMagicScrolls.Count; i++)
    //            listOfMagicScrolls[i].SetActive(false);

    //        listOfMagicScrolls[currentCount].SetActive(true);
    //    }
    //    else
    //    {
            
    //        panelTargetInfo.SetActive(false);
    //        //MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step5_done);

    //        //TutorialSteps.Instance.Step5_Done(); //если что вернуть

    //        isStepEnd = true;
    //        //HideStep();
    //        TutorialButtonClickCheck.Instance.stepNumber = 5;
    //        TutorialButtonClickCheck.Instance.CompleteStep();

    //        TutorialStep6_InfoTarget.Instance.ShowStep6();
    //    }
    //}

    //private void ToPrevPage()
    //{
    //    if (currentCount - 1 < 0) isStep = true;
    //    else currentCount--;

    //    for (var i = 0; i < listOfMagicScrolls.Count; i++)
    //        listOfMagicScrolls[i].SetActive(false);

    //    listOfMagicScrolls[currentCount].SetActive(true);
    //}

    //private void Update()
    //{
    //    //if (updateTimer)
    //    //{
    //    //    waitTime -= Time.fixedDeltaTime;
    //    //    if (waitTime <= 0)
    //    //    {
    //    //        //HideStep();
    //    //        ToNextPage();

    //    //        if (isStep)
    //    //        {
    //    //            MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step5_done);
    //    //            HideStep();
    //    //        }

    //    //        //BoolStepVarOff();
    //    //    }
    //    //    //timerImage.fillAmount = waitTime / totalTime;
    //    //    //TimerText.text = waitTime.ToString("##");

    //    //}
    //    //if (TutorialSteps.Instance.isStep4_Complete && !TutorialSteps.Instance.isStep5_Complete && isStepEnd == false)
    //    //{ 
    //    //    //if (Finch.FinchInput.GetTouchpadEvent(Node, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeRight))
    //    //    //{
    //    //    //    ToNextPage();
    //    //    //}
    //    //}
    //}

    //public void ShowStep5()
    //{
    //    objectTutorialSteps.SetActive(true);
    //    //Debug.Log("Step5_1");
    //    //HideAllPanels();

    //    //objectTutorialSteps.transform.localScale = new Vector3(0.125f, 0.125f, 1f);

    //    panelTargetInfo.SetActive(true);
    //    //Debug.Log("Step5_2");
    //    listOfMagicScrolls[0].SetActive(true);
    //    //Debug.Log("Step5_3");
    //    objectTutorialSteps.transform.position = new Vector3(PlacementController.Instance.transform.localPosition.x, Camera.main.transform.position.y, PlacementController.Instance.transform.localPosition.z);

    //    objectTutorialSteps.transform.LookAt(Camera.main.transform);
    //    objectTutorialSteps.transform.localRotation *= Quaternion.Euler(0, 180, 0);

    //    //StageText.text = stage.ToString();
    //    //TimeText.text = time.ToString();
    //    //TargetsText.text = targets.ToString();
    //    //isStep = true;


    //    //animator.Play("OpenScore");

    //    //waitTime = showTimer;
    //    //totalTime = showTimer;
    //    //updateTimer = true;
    //}

    public void ShowStep5(int listIndex)
    {
        Debug.Log("1");
        objectTutorialSteps.SetActive(true);
        //Debug.Log("2");
        panelTargetInfo.SetActive(true);
        //Debug.Log("3");
        //listOfMagicScrolls[listIndex].SetActive(true);
        Debug.Log("4");
        //objectTutorialSteps.transform.position = new Vector3(PlacementController.Instance.transform.localPosition.x, Camera.main.transform.position.y, PlacementController.Instance.transform.localPosition.z);
        Debug.Log("5");
        //objectTutorialSteps.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        objectTutorialSteps.transform.LookAt(Camera.main.transform);
        Debug.Log("6");
        //objectTutorialSteps.transform.localRotation *= Quaternion.Euler(0, 180, 0);
        objectTutorialSteps.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
        //transform.Rotate
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

    //private void HideStep()
    //{
    //    //animator.Play("CloseScore");
    //    //HideAllPanels();
    //    //objectTutorialSteps.SetActive(false);        
    //    //updateTimer = false;
    //}

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        switch (gameEvent.EventName)
        {
            case "GameStarted":
                if (ProgressionManager.Instance.currentStage == 1)
                {
                    TutorialAnimation.SetTrigger("Step1");
                    Debug.Log("*** curStage = 1");
                    //HideAllPanels();
                    ShowStep5(0);
                    MagicBookStartAnimation.Instance.BookStartAnimation();
                    StartCoroutine(AnimationWait());
                }                
                break;

            case "NewStageStarted":
                //if (ProgressionManager.Instance.currentStage == 2)
                //{
                //    TutorialAnimation.SetTrigger("Step2");
                //    Debug.Log("*** curStage = 2");
                //    //HideAllPanels();
                //    ShowStep5(1);                    
                //}

                if (ProgressionManager.Instance.currentStage == 2)
                {
                    TutorialAnimation.SetTrigger("Step3");
                    Debug.Log("*** curStage = 3");
                    //HideAllPanels();
                    ShowStep5(2);                    
                }


                if (ProgressionManager.Instance.currentStage == 3)
                {
                    TutorialAnimation.SetTrigger("Blank");
                    TutorialSteps.Instance.Step5_Done();
                    isStepEnd = true;
                    TutorialSteps.Instance.Step6_Done();
                }
                break;
            default:
                break;
        }
    }

    IEnumerator AnimationWait()
    {
        yield return new WaitForSeconds(6f);
        TutorialAnimation.SetTrigger("Step2");
        Debug.Log("*** curStage = 2");
        //HideAllPanels();
        ShowStep5(1);
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
    }
}
