using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandInitializator : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public Transform wandTransform;
    public Transform wandLeftHand;
    public Transform wandRightHand;
    public Transform wandNoDof;

    private Finch.Chirality currentPosition = Finch.Chirality.Both;
    private Finch.Chirality thisFramePosition = Finch.Chirality.None;

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        Finch.FinchCalibration.OnCalibrationEnd += SendEvent;
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
        Finch.FinchCalibration.OnCalibrationEnd -= SendEvent;
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        if (eventType.EventName == StaticEvents.calibrationStageEnded.EventName)
        {
            InitializeWand();
        }
    }

    public void SendEvent()
    {
        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.calibrationStageEnded);
    }

    public void InitializeWand()
    {
        if (Finch.FinchController.GetController(Finch.Chirality.Right).IsConnected)
            thisFramePosition = Finch.Chirality.Right;
        else if (Finch.FinchController.GetController(Finch.Chirality.Left).IsConnected)
            thisFramePosition = Finch.Chirality.Left;
        else
            thisFramePosition = Finch.Chirality.None;

        if (thisFramePosition != currentPosition)
        {
            switch (thisFramePosition)
            {
                case Finch.Chirality.Right:
                    ChangePosition(wandRightHand);
                    wandNoDof.gameObject.SetActive(false);
                    break;

                case Finch.Chirality.Left:
                    ChangePosition(wandLeftHand);
                    wandNoDof.gameObject.SetActive(false);
                    break;

                case Finch.Chirality.None:
                    ChangePosition(wandNoDof);
                    wandTransform.localPosition -= Vector3.forward * 0.2f;
                    break;
            }
        }

        wandTransform.gameObject.SetActive(true);

        this.enabled = false;
    }

    private void ChangePosition(Transform transform)
    {
        wandTransform.parent = transform;
        wandTransform.localPosition = Vector3.zero;
        wandTransform.localRotation = Quaternion.Euler(0, 0, 0);
        currentPosition = thisFramePosition;
    }

}
