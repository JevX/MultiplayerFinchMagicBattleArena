using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class ActionOnEventStartPress : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public GameObject panelStartAndScale;

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        if (gameEvent.EventName == "StartButtonPressed")
        {
            panelStartAndScale.SetActive(false);
        }
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
