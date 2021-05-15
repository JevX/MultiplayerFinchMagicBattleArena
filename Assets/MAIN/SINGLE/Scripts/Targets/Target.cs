using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using Wand;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class Target : MonoBehaviour, MMEventListener<MMGameEvent>
{
    private Health myHealth;
    private bool canChangeVulnerability;

    public HealthUI healthSlider;

    [SerializeField] private int numberInArray;

    //[FMODUnity.EventRef]
    //public string FMODKineticHit;

    //private FMOD.Studio.EventInstance FMODInstance;

    private void Awake()
    {
        myHealth = GetComponent<Health>();
    }

    public void DamageTarget(int amount, MagicType magicType)
    {
        //if (magicType == MagicType.Kinetic)
        //{
        //    FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODKineticHit);
        //    FMODInstance.start();
        //    FMODInstance.release();
        //}

        myHealth.Damage(amount, magicType);

        UpdateHealth();

        if (canChangeVulnerability && myHealth.vulnerability == magicType)
        {
            MagicType nextVulnerability = MagicType.None;
            switch (myHealth.vulnerability)
            {
                case MagicType.Electro:
                    nextVulnerability = MagicType.Fire;
                    break;
                case MagicType.Fire:
                    nextVulnerability = MagicType.Ice;
                    break;
                case MagicType.Ice:
                    nextVulnerability = MagicType.Kinetic;
                    break;
                case MagicType.Kinetic:
                    nextVulnerability = MagicType.Electro;
                    break;
            }

            SetVulnerability(nextVulnerability);
        }
    }

    public void CanChangeVulnerability(bool value)
    {
        canChangeVulnerability = value;
    }

    public void UpdateHealth()
    {
        healthSlider.UpdateValue(myHealth.GetHealthValue());
    }

    public void SetMaximumHealth(int amount)
    {
        myHealth.maximumHealth = amount;
    }

    public void SetVulnerability(MagicType magicType)
    {
        myHealth.vulnerability = magicType;

        healthSlider.VulnerabilityChange(magicType);
    }

    public void Kill()
    {
        myHealth.Kill();
    }

    public void SetNumberInArray(int value)
    {
        numberInArray = value;
    }

    public int GetNumberInArray()
    {
        return numberInArray;
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        healthSlider.gameObject.SetActive(true);
        myHealth.Revive();
        UpdateHealth();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
        if (myHealth.IsDead)
        {
            MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.targetKilled);
            TargetsManager.Instance.SetTargetAsInactive(this);
        }
    }
}
