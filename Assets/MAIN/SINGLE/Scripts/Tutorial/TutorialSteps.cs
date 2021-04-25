using UnityEngine;
using MoreMountains.Tools;
using Finch;
using System.Collections;

public class TutorialSteps : MMSingleton<TutorialSteps>, MMEventListener<MMGameEvent>
{
    public bool isStep1_Complete = false;
    public bool isStep2_Complete = false;
    public bool isStep3_Complete = false;
    public bool isStep4_Complete = false;
    public bool isStep5_Complete = false;
    public bool isStep6_Complete = false;
    

    public GameObject step1_infoPanel;
    public GameObject step2_infoPanel;
    public GameObject step3_infoPanel;
    public GameObject step4_infoPanel;
    public GameObject step5_infoPanel;
    public GameObject step6_infoPanel;


    public GameObject panelComplete;
    public GameObject imageOk;

    Coroutine mainCoroutine;

    private void Start()
    {
        AllPanelClose();
        Step1_Start();
    }   

    public void Step1_Start() //старт калибровки
    {
        step1_infoPanel.SetActive(true);
    }

    public void Step2_Start() //старт сканирования пространства
    {
        step2_infoPanel.SetActive(true);
    }

    public void Step3_Start() //старт выбора места для установки поля игры
    {
        step3_infoPanel.SetActive(true);
    }

    public void Step4_Start() //старт для масштабирования поля или старта игры
    {
        step4_infoPanel.SetActive(true);
    }

    public void Step5_Start() //старт для рассказа про таргеты
    {
        step5_infoPanel.SetActive(true);
    }

    public void Step6_Start() //старт для рассказа про книгу
    {
        step6_infoPanel.SetActive(true);
    }

    public void AllPanelClose()
    {
        step1_infoPanel.SetActive(false);
        step2_infoPanel.SetActive(false);
        step3_infoPanel.SetActive(false);
        step4_infoPanel.SetActive(false);
        step5_infoPanel.SetActive(false);
        step6_infoPanel.SetActive(false);
    }

    public void Step1_Done() //для калибровки
    {
        isStep1_Complete = true;
        isStep2_Complete = false;
        isStep3_Complete = false;
        isStep4_Complete = false;
        isStep5_Complete = false;
        isStep6_Complete = false;        

        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step1_done);
        ShowStepComplete();
        AllPanelClose();
        Step2_Start();
    }
    
    public void Step2_Done() //для сканирования пространства
    {
        isStep2_Complete = true;       
        isStep3_Complete = false;
        isStep4_Complete = false;
        isStep5_Complete = false;
        isStep6_Complete = false;        

        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step2_done);
        ShowStepComplete();
        AllPanelClose();
        Step3_Start();
    }

    public void Step3_Done() //для выбора места для установки сцены
    {
        isStep3_Complete = true;        
        isStep4_Complete = false;
        isStep5_Complete = false;
        isStep6_Complete = false;

        ShowStepComplete();

        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step3_done);
        AllPanelClose();
        Step4_Start();
    }

    public void Step4_Done() //изменение масштаба или старта
    {
        isStep4_Complete = true;    
        isStep5_Complete = false;
        isStep6_Complete = false;

        ShowStepComplete();

        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step4_done);
        AllPanelClose();
        Step5_Start();
    }

    public void Step5_Done() //рассказ про таргеты
    {
        isStep5_Complete = true;
        isStep6_Complete = false;

        ShowStepComplete();

        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step5_done);
        AllPanelClose();
        Step6_Start();
    }

    public void Step6_Done() //рассказ про книгу
    {
        isStep6_Complete = true;

        ShowStepComplete();

        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step6_done);
        AllPanelClose();
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        switch (gameEvent.EventName)
        {
            case "Step1_Done":
                AllPanelClose();
                Step2_Start();
                break;
            case "Step2_Done":
                AllPanelClose();
                Step3_Start();
                break;
            case "Step3_Done":
                AllPanelClose();
                Step4_Start();
                break;
            case "Step4_Done":
                AllPanelClose();
                Step5_Start();
                break;
            case "Step5_Done":
                AllPanelClose();
                Step6_Start();
                break;
            case "Step6_Done":
                AllPanelClose();
                break;

            default:
                break;
        }

        if (gameEvent.EventName == StaticEvents.calibrationStageEnded.EventName)
        {
            TutorialButtonClickCheck.Instance.stepNumber = 1;
            TutorialButtonClickCheck.Instance.CompleteStep();
            //Step1_Done(); // если что вернуть
        }
    }

    private void ShowStepComplete()
    {
        if (mainCoroutine != null)
            StopCoroutine(mainCoroutine);
        mainCoroutine = StartCoroutine(ShowCompleteImage());
    }

    private IEnumerator ShowCompleteImage()
    {
        panelComplete.SetActive(true);
        imageOk.SetActive(true);
        //if (stepNumber != 3)
        yield return new WaitForSeconds(2f);
        imageOk.SetActive(false);
        panelComplete.SetActive(false);
        //StopCoroutine(ShowCompleteImage());
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
