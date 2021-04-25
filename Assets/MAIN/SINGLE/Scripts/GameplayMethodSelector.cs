using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class GameplayMethodSelector : MonoBehaviour, MMEventListener<MMGameEvent>
{

    public GameObject FinchCalibration;


    public bool UseMethodSelector;

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        if (!UseMethodSelector)
        {
            UseController();
        }
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
    }

    public void OnMMEvent(MMGameEvent eventType)
    {

    }

    public void UseController()
    {
        FinchCalibration.SetActive(true);
        Vector3 finchPosition = Camera.main.transform.position + Camera.main.transform.forward * 10f;
        finchPosition.y = Camera.main.transform.position.y;
        FinchCalibration.transform.position = finchPosition;
        FinchCalibration.transform.forward = Camera.main.transform.forward;
        FinchCalibration.transform.rotation = Quaternion.Euler(0, FinchCalibration.transform.eulerAngles.y, 0);

        gameObject.SetActive(false);
    }

    public void UseHID()
    {
        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.calibrationStageEnded);
        gameObject.SetActive(false);
    }
}
