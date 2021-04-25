using Finch;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс для управления палочкой. Черновой вариант.
/// </summary>
public class WandInput : MonoBehaviour, MMEventListener<MMGameEvent>
{
    private MagicWand myWand;
    bool gameStared = false;

    bool castBegin;

    public float holdTime = 0.3f;
    bool canBeginCast;

    Coroutine WaitCastCoroutine;

    public MotionEventListener motionEventListener;
    public void OnMMEvent(MMGameEvent eventType)
    {
        if (eventType.EventName == StaticEvents.gameStarted.EventName)
        {
            gameStared = true;
        }

        if (!gameStared)
            return;

        if (eventType.EventName == StaticEvents.motionThresholdCrossed.EventName)
        {
            if (!myWand.isDrawing && !castBegin)
            {
                //Debug.Log("Cast from motion begin");
                BeginCast();
            }
        }

        if (eventType.EventName == StaticEvents.motionStopped.EventName)
        {
            if (!myWand.isDrawing)
            {
                //Debug.Log("Cast from motion end");

                EndCast();
            }
        }
    }

    private void CheckMotion()
    {
        Vector3 sampledMotion = MotionEventListener.Instance.GetMotionVector();
        sampledMotion = Camera.main.transform.InverseTransformDirection(sampledMotion);

        Debug.Log(sampledMotion);

        if (Mathf.Abs(sampledMotion.x) > 0.3)
        {
            if (sampledMotion.x > 0)
                Debug.Log("Motion right");
            else
                Debug.Log("Motion left");
        }
        if (Mathf.Abs(sampledMotion.y) > 0.3)
        {
            if (sampledMotion.y > 0)
                Debug.Log("Motion up");
            else
                Debug.Log("Motion down");
        }
        if (Mathf.Abs(sampledMotion.z) > 0.3)
        {
            if (sampledMotion.z > 0)
                Debug.Log("Motion forward");
            else
                Debug.Log("Motion backward");
        }
    }


    public void RegisterMe(MagicWand myWand)
    {
        this.myWand = myWand;
    }

    private void Awake()
    {
        gameStared = false;
    }

    private bool CheckSwipe()
    {
        return FinchInput.GetTouchpadEvent(NodeType.LeftHand, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeLeft) ||
        FinchInput.GetTouchpadEvent(NodeType.LeftHand, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeRight) ||
        FinchInput.GetTouchpadEvent(NodeType.RightHand, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeLeft) ||
        FinchInput.GetTouchpadEvent(NodeType.RightHand, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeRight);
    }

    private void Update()
    {
        if (!gameStared)
            return;

        if (CheckSwipe())
        {
            myWand.ClearAndStopDrawing();
        }


        //Игрок впервые опустил палец на тач.
        if (FinchController.GetPressDown(Chirality.Any, RingElement.Touch) || HIDController.Instance.OnPressDownTimes(1))
        {
            Debug.Log("Finger pressed down TOUCH");
            StartClickTest();
        }

        //Обрабатываем нажатие до клика
        if (FinchController.GetPressDown(Chirality.Any, RingElement.HomeButton) || HIDController.Instance.OnPressDownTimes(2))
        {
            if (castBegin)
            {
                EndCast();
                return;
            }

            StopClickTest();

            Debug.Log("Finger pressed down CLICK");
            //Начинаем рисовать. Можно заменить ивентом, что начинает рисовать по таймеру превышающему минимальное время нажатия.
            myWand.StartDraw();
        }


        //Когда игрок поднимает палец.
        if (FinchController.GetPressUp(Chirality.Any, RingElement.HomeButton) || FinchController.GetPressUp(Chirality.Any, RingElement.Touch) || HIDController.Instance.OnPressUp())
        {
            StopClickTest();

            if (castBegin)
            {
                Debug.Log("Finger pressed up CLICK");
                EndCast();
                return;
            }
            else
            {
                Debug.Log("Finger pressed Up TOUCH");
                myWand.EndDraw();
            }
        }

    }

    private void StartClickTest() 
    {
        StopClickTest();
        WaitCastCoroutine = StartCoroutine(ClickTest());
    }

    private void StopClickTest()
    {
        if (WaitCastCoroutine != null)
            StopCoroutine(WaitCastCoroutine);
    }

    IEnumerator ClickTest()
    {
        yield return new WaitForSeconds(holdTime);
        BeginCast();
    }

    public void BeginCast()
    {
        castBegin = true;
        //Очищаем рисунок, на случай если игрок начнет двигать пальцем.
        myWand.ClearAndStopDrawing();
        //Запускаем событие начала магии. Обычно это эффекты заряда магии. Сюда можно привязать механизмы именно чарджа магии.
        myWand.BeginMagic();
    }

    public void EndCast()
    {
        if (!castBegin)
            return;

        castBegin = false;
        //Очищаем рисунок, на случай если игрок начнет двигать пальцем.
        myWand.ClearAndStopDrawing();
        //Заканчиваем каст магии. Мы молодцы.
        myWand.CastDone();
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
        //motionEventListener.eventOnEnterThresholdVelocity.AddListener(BeginCast);
        //motionEventListener.eventOnMotionStop.AddListener(EndCast);
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
       // motionEventListener.eventOnEnterThresholdVelocity.RemoveListener(BeginCast);
       // motionEventListener.eventOnMotionStop.RemoveListener(EndCast);
    }
}
