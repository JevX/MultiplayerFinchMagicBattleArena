using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class TargetShield : MMSingleton<TargetShield>, MMEventListener<MMGameEvent>
{
    public GameObject targetShield;
    public float shieldTimerShow = 2f;

    [FMODUnity.EventRef]
    public string FMODActivateShield;
    private FMOD.Studio.EventInstance FMODInstance;

    public void OnDamageInShield()
    {
        Debug.Log("Shield On");
        targetShield.SetActive(true);
        Invoke("HideTargetShield", shieldTimerShow);

        FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODActivateShield);
        FMODInstance.start();
        FMODInstance.release();
    }

    void HideTargetShield()
    {
        Debug.Log("Shield Off");
        targetShield.SetActive(false);
    }

    //private void OnEnable()
    //{
    //    Debug.Log("Shield Write On");
    //    Health.OnTargetDamagedInShield += OnDamageInShield;
    //}

    //private void OnDisable()
    //{
    //    Debug.Log("Shield Write Off");
    //    Health.OnTargetDamagedInShield -= OnDamageInShield;
    //}

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        if (gameEvent.EventName == "targetShieldDamaged")
        {
            //Debug.Log("1-1");
            OnDamageInShield();
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
