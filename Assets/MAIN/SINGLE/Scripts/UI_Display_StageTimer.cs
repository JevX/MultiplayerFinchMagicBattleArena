using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Display_StageTimer : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public Text Timer;

    float timer;

    bool stopped = true;

    public void StartTimer()
    {
        stopped = false;
    }

    public void StopTimer()
    {
        stopped = true;
    }

    public void ResetTimer()
    {
        timer = 0;
    }

    private void FixedUpdate()
    {
        if (!stopped)
        {
            timer += Time.fixedDeltaTime;
            Timer.text = MoreMountains.Tools.MMTime.FloatToTimeString(timer);
        }

    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        //if (gameEvent.EventName == StaticEvents.gameStarted.EventName)
        //{
        //    StartTimer();
        //}
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
