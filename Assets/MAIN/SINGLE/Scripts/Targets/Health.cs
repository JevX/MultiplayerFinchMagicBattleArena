using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Tools;

public class Health : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public int currentHealth;
    public float partialHealth;
    public int maximumHealth;

    public float nonVulnerableDamageModifier;
    public MagicType vulnerability;

    public float WaitBeforeDeath;

    private Coroutine killCoroutine;

    private bool dead;

    private TargetActionController targetActionController;

    private TargetShield shield;

    public bool IsDead { get => dead; set { } }

    private void Awake()
    {
        targetActionController = GetComponent<TargetActionController>();
        shield = GetComponent<TargetShield>();
    }
    /// <summary>
    /// Нанесение урона объекту
    /// </summary>
    /// <param name="damageAmount">Объем нанесенного урона</param>
    /// <param name="magicType">Тип урона</param>
    public void Damage(int damageAmount, MagicType magicType)
    {
        if (dead) //Если объект уже уничтожен, не обрабатываем действие.
            return;

        if (vulnerability != MagicType.None) //Проверяем тип уязвимости объекта. При его отсутствии просто наносим урон
        {
            if (magicType == vulnerability) //Если тип урона совпадает с типом уязвимости, наносим урон
                currentHealth -= damageAmount;
            else //В обработм случае наноим частичный урон.
            {
                HandlePartialDamage(damageAmount * nonVulnerableDamageModifier);
                //Debug.Log("111 OnTargetDamagedInShield();");

                shield.OnDamageInShield();

                //MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.targetShieldDamaged); //щит срабатывает на всех

                //MMEventManager.TriggerEvent(new MMGameEvent("targetShieldDamaged"));
                //TargetDamagedInShield?.Invoke();
            }
        }
        else //Происходит только при отсутсвии уязвимостей у цели.
        {
            currentHealth -= damageAmount;
        }

        //TargetDamaged?.Invoke(); //Вот тут событие

        //if (OnTargetDamaged != null)
        //    OnTargetDamaged();

        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.targetDamaged);

        targetActionController.PlayParticleHitFx();
        targetActionController.PlayRandomAnimationsHit();

        HandleLethalDamage(); //Проверяем являлся ли урон летальным для цели.
    }

    /// <summary>
    /// Обработка частичного урона.
    /// </summary>
    /// <param name="damage"></param>
    private void HandlePartialDamage(float damage)
    {
        if (partialHealth == 0)
        {
            currentHealth -= 1;
            partialHealth = 1;
        }

        partialHealth -= damage;        

        if (partialHealth <= 0)
        {
            partialHealth = 0;
        }
    }

    /// <summary>
    /// Здесь проверяем факт нанесения урона по цели.
    /// </summary>
    private void HandleLethalDamage()
    {
        //Если здоровье цели опустилось до нуля.
        //currentHealth -1 обрабатывается в том случае, когда мы нанесли цельный урон
        //При состоянии currentHealth = 0 и partialHealth > 0
        if (currentHealth == -1 || (currentHealth <= 0 && partialHealth <= 0))
        {
            Kill();
        }
    }

    /// <summary>
    /// "Воскрешение" цели
    /// </summary>
    public void Revive()
    {
        StopKillCoroutine();

        dead = false;
        currentHealth = maximumHealth;
        partialHealth = 0;

        if (targetActionController)
            targetActionController.ResetDummy();
    }

    /// <summary>
    /// Стандартная обработка смерти цели. Запускает время ожидания перед смертью, если оно есть.
    /// </summary>
    public void Kill()
    {
        currentHealth = 0;
        partialHealth = 0;

        dead = true;

        targetActionController.KillTarget();

        if (WaitBeforeDeath > 0)
        {
            StartCoroutine(TimerBeforeDeath(WaitBeforeDeath));
        }
        else InstantKill();
    }

    /// <summary>
    /// Моментально убивает цель. 
    /// </summary>
    public void InstantKill()
    {
        StopKillCoroutine();

        //if (OnTargetDead != null)
        //    OnTargetDead();

        //TargetDead?.Invoke();

        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.targetDead);

        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Прекращает обработку времени ожидания перед смертью.
    /// </summary>
    private void StopKillCoroutine()
    {
        if (killCoroutine != null)
        {
            StopCoroutine(killCoroutine);
            killCoroutine = null;
        }
    }

    public float GetHealthValue()
    {
        return (currentHealth + partialHealth) / maximumHealth;
    }

    IEnumerator TimerBeforeDeath(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        InstantKill();
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {

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
