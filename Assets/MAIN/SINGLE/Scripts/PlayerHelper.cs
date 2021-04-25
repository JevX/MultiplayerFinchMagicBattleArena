using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHelper : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public float PlayerNotActiveInSeconds = 30;
    public Animator playerHelpAnimator;
    float timeBeforeHelpPlayer;
    public GameObject objectTutorialSteps;

    bool HelpCanBeUsed;

    private void Awake()
    {
        HelpCanBeUsed = false;
        timeBeforeHelpPlayer = -1;
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        if (HelpCanBeUsed)
        {
            if (eventType.EventName == StaticEvents.targetKilled.EventName)
            {
                if (timeBeforeHelpPlayer <= 0)
                {
                    HidePlayerHelp();
                }

                timeBeforeHelpPlayer = PlayerNotActiveInSeconds;
            }

        }
        else if (eventType.EventName == StaticEvents.stageStarted.EventName)
        {
            if (ProgressionManager.Instance.currentStage == 3)
            {
                HelpCanBeUsed = true;
                timeBeforeHelpPlayer = PlayerNotActiveInSeconds;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!ScoreManager.Instance.GameEnded)
        {
            if (timeBeforeHelpPlayer > 0)
            {
                timeBeforeHelpPlayer -= Time.fixedDeltaTime;
                if (timeBeforeHelpPlayer <= 0)
                {
                    ShowPlayerHelp();
                }
            }
        }

    }

    private void ShowPlayerHelp()
    {
        objectTutorialSteps.transform.position = new Vector3(PlacementController.Instance.transform.localPosition.x, Camera.main.transform.position.y, PlacementController.Instance.transform.localPosition.z);
        Debug.Log("5");
        objectTutorialSteps.transform.LookAt(Camera.main.transform);
        Debug.Log("6");
        objectTutorialSteps.transform.Rotate(new Vector3(0f, 180f, 0f), Space.Self);
        playerHelpAnimator.SetTrigger("Step2");
    }

    private void HidePlayerHelp()
    {
        playerHelpAnimator.SetTrigger("Blank");
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
