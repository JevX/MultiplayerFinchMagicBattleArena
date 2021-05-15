using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс статических ивентов для игровых событий.
/// </summary>
public static class StaticEvents
{
    public static readonly MMGameEvent allTargetsKilled = new MMGameEvent("AllTargetsKilled"); //Используется
    public static readonly MMGameEvent targetKilled = new MMGameEvent("TargetKilled"); //Используется
    public static readonly MMGameEvent gameStarted = new MMGameEvent("GameStarted"); //Используется
    public static readonly MMGameEvent stageEnded = new MMGameEvent("StageEnded"); //Используется
    public static readonly MMGameEvent stageStarted = new MMGameEvent("NewStageStarted"); //Используется

    public static readonly MMGameEvent fireMagic = new MMGameEvent("FireMagic");
    public static readonly MMGameEvent IceMagic = new MMGameEvent("IceMagic");
    public static readonly MMGameEvent electroMagic = new MMGameEvent("ElectroMagic");
    public static readonly MMGameEvent kineticMagic = new MMGameEvent("KineticMagic");
    public static readonly MMGameEvent magicChanged = new MMGameEvent("MagicChanged");
    
    public static readonly MMGameEvent motionThresholdCrossed = new MMGameEvent("MotionThresholdCrossed"); //Используется
    public static readonly MMGameEvent motionStopped = new MMGameEvent("MotionStopped"); //Используется

    public static readonly MMGameEvent targetShieldDamaged = new MMGameEvent("targetShieldDamaged");
    public static readonly MMGameEvent targetDamaged = new MMGameEvent("TargetDamaged");
    public static readonly MMGameEvent targetDead = new MMGameEvent("TargetDead");

    public static readonly MMGameEvent startButtonPressed = new MMGameEvent("StartButtonPressed");

    public static readonly MMGameEvent calibrationStageEnded = new MMGameEvent("CalibrationStageEnded");

    public static readonly MMGameEvent step1_done = new MMGameEvent("Step1_Done");
    public static readonly MMGameEvent step2_done = new MMGameEvent("Step2_Done");
    public static readonly MMGameEvent step3_done = new MMGameEvent("Step3_Done");
    public static readonly MMGameEvent step4_done = new MMGameEvent("Step4_Done");
    public static readonly MMGameEvent step5_done = new MMGameEvent("Step5_Done");
    public static readonly MMGameEvent step6_done = new MMGameEvent("Step6_Done");
}
