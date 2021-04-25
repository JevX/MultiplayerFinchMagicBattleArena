using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class MagicScollOnScreenSwitch : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public GameObject panelMagicScollOnScreen;

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        if (gameEvent.EventName == "GameStarted")
        {
            //panelMagicScollOnScreen.SetActive(true);
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
