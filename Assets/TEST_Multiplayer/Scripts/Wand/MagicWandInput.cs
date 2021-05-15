using System.Collections;
using MAIN.InputSystem;
using MoreMountains.Tools;
using Wand;
using UnityEngine;

namespace MAIN.Wand
{
    public class MagicWandInput : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        private MagicWandController MyWand => MagicWandController.Instance;
        
        bool gameStared = true; //TODO

        bool castBegin;

        public float holdTime = 0.3f;
        bool canBeginCast;

        Coroutine WaitCastCoroutine;

        public MotionEventListener motionEventListener;

        public void OnMMEvent(MMGameEvent eventType)
        {

            if (eventType.EventName == StaticEvents.gameStarted.EventName) gameStared = true;

            if (!gameStared) return;

            if (eventType.EventName == StaticEvents.motionThresholdCrossed.EventName)
            {
                if (!MyWand.isDrawing && !castBegin)
                {
                    //Debug.Log("Cast from motion begin");
                    BeginCast();
                }
            }
            else if (eventType.EventName == StaticEvents.motionStopped.EventName)
            {
                if (!MyWand.isDrawing)
                {
                    //Debug.Log("Cast from motion end");

                    EndCast();
                }
            }
        }
        
        private void Update()
        {
            if (!gameStared) return;

            if (UserInputController.Instance.CheckSwipe())
            {
                MyWand.ClearAndStopDrawing();
            }

            //Игрок впервые опустил палец на тач.
            if (UserInputController.Instance.GetPressDownTouch())
            {
                Debug.Log("Finger pressed down TOUCH");
                StartClickTest();
            }

            //Обрабатываем нажатие до клика
            if (UserInputController.Instance.GetPressDownHome())
            {
                if (castBegin)
                {
                    EndCast();
                    return;
                }

                StopClickTest();

                Debug.Log("Finger pressed down CLICK");
                //Начинаем рисовать. Можно заменить ивентом, что начинает рисовать по таймеру превышающему минимальное время нажатия.
                MyWand.StartDraw();
            }


            //Когда игрок поднимает палец.
            if (UserInputController.Instance.GetPressUpHome() || UserInputController.Instance.GetPressUpTouch())
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
                    MyWand.EndDraw();
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
            MyWand.ClearAndStopDrawing();
            //Запускаем событие начала магии. Обычно это эффекты заряда магии. Сюда можно привязать механизмы именно чарджа магии.
            MyWand.BeginMagic();
        }

        public void EndCast()
        {
            if (!castBegin)return;

            castBegin = false;
            //Очищаем рисунок, на случай если игрок начнет двигать пальцем.
            MyWand.ClearAndStopDrawing();
            //Заканчиваем каст магии. Мы молодцы.
            MyWand.CastDone();
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
}