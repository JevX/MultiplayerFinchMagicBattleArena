using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MMSingleton<ProgressionManager>, MMEventListener<MMGameEvent>
{
    public int currentStage;
    public float waitBetweenStages = 5;
    public float koefPositionRandomRadius = 1;
    public ProgressionData progressData;
    public ProgressionData.StageData stageData;

    private float stageTimer;

    protected override void Awake()
    {
        base.Awake();
        currentStage = 0;
    }

    private void Start()
    {
        //GenerateTargetsBasedOnStage();
    }

    private void FixedUpdate()
    {
        stageTimer += Time.fixedDeltaTime;
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        if (gameEvent.EventName == "AllTargetsKilled")
        {
            ScoreManager.Instance.ShowStageResults(stageData.stageName,
                                                    stageTimer,
                                                    stageData.standartTargets + stageData.specialTargets);

            if (stageData.stageName != "Final Stage") StartWaitBetweenStages();
        }

        if(gameEvent.EventName == StaticEvents.gameStarted.EventName)
        {
            StartNewStage();
        }
    }


    public void StartWaitBetweenStages()
    {
        StartCoroutine(WaitBetweenStages(waitBetweenStages));
    }

    public void ResetGame()
    {
        currentStage = 2;
        StartNewStage();
    }

    /// <summary>
    /// Корутина для отсчета времени между этапами.
    /// </summary>
    /// <param name="seconds">Секунды</param>
    /// <returns></returns>
    IEnumerator WaitBetweenStages(float seconds)
    {
        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.stageEnded);
        yield return new WaitForSeconds(seconds);
        StartNewStage();
    }

    /// <summary>
    /// Увеличивает счетчик стадии и запускает новую. НЕ очищает поле от уже уничтоженных противников.
    /// </summary>
    public void StartNewStage()
    {
        currentStage++;
        stageTimer = 0;

        stageData = progressData.GetStageSettings(currentStage);
        if (stageData.stageName != "Tutorial")
        {
            ScoreManager.Instance.UpdateStageInfo(stageData.stageName);
            ScoreManager.Instance.EndStage();
        }

        GenerateTargets(stageData.standartTargets, stageData.minimumHealth, stageData.maximumHealth, false, stageData.chanceToHaveAShield);
        GenerateTargets(stageData.specialTargets, stageData.specialTargetMinimumHealth, stageData.specialTargetMaximumHealth, true, stageData.chanceToHaveAShield);
        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.stageStarted);
    }

    /// <summary>
    /// Генерирует на поле боя цели.
    /// </summary>
    /// <param name="amount">Количество</param>
    /// <param name="minimumHealth">Минимум здоровья</param>
    /// <param name="maximumHealth">Максимум здоровья</param>
    /// <param name="special">Является ли цель специальной (переключается защита)</param>
    /// <param name="shieldChanceData">Шанс того, что у цели будет щит (не работает для специальной)</param>
    private void GenerateTargets(int amount, int minimumHealth, int maximumHealth, bool special, float shieldChanceData)
    {
        for (int i = 0; i < amount; i++)
        {
            float shieldChance = Random.Range(0, 1f);

            MagicType targetMagicType = MagicType.None;

            if (special || shieldChance < shieldChanceData)
            {
                targetMagicType = (MagicType)Random.Range(1, 5);
            }

            TargetsManager.Instance.GenerateTarget
            (
                targetMagicType, // <param name="magicVulnerabilityType">Тип уязвимости цели</param>
                Random.Range(minimumHealth, maximumHealth + 1), // <param name="healthAmount">Стартовое количество здоровья</param>
                special
            );
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
