using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using Wand;
using UnityEngine;

public class DamagingObject : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public MagicType magicType;
    public DamageMethod damageMethod;
    public int damage;

    [Header("Settings disable by collisions")]
    public bool disableAfterCollision;
    public int collisionTimesBeforeDisable = 3;
    public bool instantDisableIfCollideOnDamageable;

    private int timesCollided;

    [Header("What objects we trying to damage")]
    public bool DamageTrigger;
    public bool DamageCollision;

    [Header("Disabling timers")]
    public bool startTimerAfterEnabled;
    public bool startTimerWhenThrowed;
    public float disableTimer = 5;

    [Header("Disabling By Events")]
    public bool disableOnStageEnd;

    bool throwed;
    Coroutine myTimer;

    [FMODUnity.EventRef]
    public string FMODEvent;

    public ParticleSystem particlesOnDisable;

    //private FMOD.Studio.EventInstance FMODInstance;

    public enum DamageMethod
    {
        None,
        Anytime,
        OnlyAfterThrow
    }

    private void OnEnable()
    {
        throwed = false;
        timesCollided = 0;

        if (startTimerAfterEnabled)
        {
            BeginCountdown();
        }

        this.MMEventStartListening<MMGameEvent>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (DamageCollision)
            ProcessDamageMethod(collision.gameObject);

        CheckAmountOfCollisions();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (DamageTrigger)
            ProcessDamageMethod(other.gameObject);

        //CheckAmountOfCollisions();
    }

    private void ProcessDamageMethod(GameObject target)
    {
        switch (damageMethod)
        {
            case DamageMethod.None:
                break;

            case DamageMethod.Anytime:
                TryDealDamage(target);
                break;

            case DamageMethod.OnlyAfterThrow:
                if (throwed)
                {
                    TryDealDamage(target);
                }
                break;
        }
    }

    private void TryDealDamage(GameObject target)
    {
        if (TargetsManager.Instance.DamageRegisteredTarget(damage, magicType, target) && instantDisableIfCollideOnDamageable)
        {
            if (particlesOnDisable != null)
                Instantiate(particlesOnDisable).transform.position = transform.position;
            FMODUnity.RuntimeManager.PlayOneShot(FMODEvent, gameObject.transform.position);
            gameObject.SetActive(false);
        }

    }

    private void CheckAmountOfCollisions()
    {
        if (disableAfterCollision)
        {
            timesCollided++;
            if (timesCollided > collisionTimesBeforeDisable)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void MarkObjectAsThrowed()
    {
        throwed = true;
        if (startTimerWhenThrowed)
        {
            BeginCountdown();
        }
    }

    private void BeginCountdown()
    {
        if (!gameObject.activeInHierarchy)
            return;

        ResetTimer();
        myTimer = StartCoroutine(DisableCountdown());
    }

    IEnumerator DisableCountdown()
    {
        yield return new WaitForSeconds(disableTimer);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ResetTimer();
        if (particlesOnDisable != null)

        this.MMEventStopListening<MMGameEvent>();
    }

    private void ResetTimer()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (myTimer != null)
            StopCoroutine(myTimer);
        myTimer = null;
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        if (eventType.EventName == StaticEvents.stageEnded.EventName)
        {
            if (disableOnStageEnd)
            BeginCountdown();
        }
    }
}
