using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс статических ивентов для игровых событий.
/// </summary>
public static class StaticEvents
{
    public static MMGameEvent allTargetsKilled = new MMGameEvent("AllTargetsKilled"); //Используется
    public static MMGameEvent targetKilled = new MMGameEvent("TargetKilled"); //Используется
    public static MMGameEvent gameStarted = new MMGameEvent("GameStarted"); //Используется
    public static MMGameEvent stageEnded = new MMGameEvent("StageEnded"); //Используется
    public static MMGameEvent stageStarted = new MMGameEvent("NewStageStarted"); //Используется

    public static MMGameEvent fireMagic = new MMGameEvent("FireMagic");
    public static MMGameEvent IceMagic = new MMGameEvent("IceMagic");
    public static MMGameEvent electroMagic = new MMGameEvent("ElectroMagic");
    public static MMGameEvent kineticMagic = new MMGameEvent("KineticMagic");
    public static MMGameEvent magicChanged = new MMGameEvent("MagicChanged");

    public static MMGameEvent motionThresholdCrossed = new MMGameEvent("MotionThresholdCrossed"); //Используется
    public static MMGameEvent motionStopped = new MMGameEvent("MotionStopped"); //Используется

    public static MMGameEvent targetShieldDamaged = new MMGameEvent("targetShieldDamaged");
    public static MMGameEvent targetDamaged = new MMGameEvent("TargetDamaged");
    public static MMGameEvent targetDead = new MMGameEvent("TargetDead");

    public static MMGameEvent startButtonPressed = new MMGameEvent("StartButtonPressed");

    public static MMGameEvent calibrationStageEnded = new MMGameEvent("CalibrationStageEnded");

    public static MMGameEvent step1_done = new MMGameEvent("Step1_Done");
    public static MMGameEvent step2_done = new MMGameEvent("Step2_Done");
    public static MMGameEvent step3_done = new MMGameEvent("Step3_Done");
    public static MMGameEvent step4_done = new MMGameEvent("Step4_Done");
    public static MMGameEvent step5_done = new MMGameEvent("Step5_Done");
    public static MMGameEvent step6_done = new MMGameEvent("Step6_Done");
}
