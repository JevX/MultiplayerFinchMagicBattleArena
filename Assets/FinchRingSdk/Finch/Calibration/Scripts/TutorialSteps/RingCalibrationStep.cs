// Copyright 2018 - 2020 Finch Technologies Ltd. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finch
{
	public class RingCalibrationStep : TutorialStep
	{
        public enum HandHint
        {
            Base,
            LowerHand,
            UpperHand,
            StraightHand,
            HeadHint
        }

        [Header("Tutorials")]
        public NotificationCalibration Notification;
        public NotificationWords Header;

        public GameObject CalibrateRigthHandWithoutArms;
        public GameObject CalibrateLeftHandWithoutArms;
        public GameObject CalibrateRigthHandWithArms;
        public GameObject CalibrateLeftHandWithArms;
        public GameObject ReadyStanceRight;
        public GameObject ReadyStanceLeft;

        [Header("Swap Hint")]
        public GameObject LeftSwapHint;
        public GameObject RightSwapHint;

        [Header("Errors")]
        public float ErrorDuration = 1.5f;
        public GameObject[] HorizontalError = new GameObject[5];

        [Header("Rings")]
        private const float angleBorderWide = 0.5f;

        private const float RollAngleBorderSin = 0.5f;
        private const float PitchControllerAngleBorderSin = 0.70710678118f; //Sin(45deg)

        private float timeEndError;
        protected HandHint hint;
        private bool readyToCalibrate;

        private float CurrentAngleBorder;

        private bool initialState = false;

        public override void Init(FinchCalibrationSettings settings)
        {
            base.Init(settings);
            initialState = true;
            HandedMode targetMode = FinchNodeManager.GetUpperArmCount() == 2 ? HandedMode.TwoHanded : HandedMode.OneHanded;
            Internal.Settings.ReplyManager.ChangeBodyRotationMode(targetMode);
            Internal.Settings.ReplyManager.SettingsReply += DoAfterInitialState;
            return;
        }

        private void Update()
        {
            if (!initialState)
            {
                HandleUpdate();
            }
        }

        private void DoAfterInitialState(object obj, System.EventArgs args)
        {
            initialState = false;
            settings.IsMomentalCalibration &= false;
            readyToCalibrate = false;

            if (settings.IsMomentalCalibration)
            {
                Calibrate();
            }
            else
            {
                timeEndError = 0;
                HandleUpdate();
            }

            CurrentAngleBorder = CalibrationHorizontalError.AngleBorderSin;
        }

        private void HandleUpdate()
        {
            UpdateChirality();
            UpdateSprite();
            TryCalibrate();
        }

        protected virtual void TryCalibrate()
        {
            RingElement button = RingElement.HomeButton;

            readyToCalibrate &= Time.time > timeEndError;
            readyToCalibrate |= !FinchController.GetPress(Chirality.Any, button);
            readyToCalibrate |= Time.time < timeEndError && GetHandAngle() == HandHint.Base;

            if (FinchController.GetPressDown(FinchNodeManager.GetControllerConectionChirality(), button))
            {
                hint = GetHandAngle();
                if (hint != HandHint.Base)
                {
                    timeEndError = Time.time + ErrorDuration;
                }
            }

            if (FinchController.GetPressTime(FinchNodeManager.GetControllerConectionChirality(), button) > settings.TimePressingToCallCalibration && Time.time > timeEndError && readyToCalibrate)
            {
                if (Mathf.Abs((float)Internal.Calibration.Calculations.GetHmdPitchAngle()) * Mathf.Rad2Deg > 45)
                {
                    return;
                }

                hint = GetHandAngle();

                if (hint == HandHint.Base)
                {
                    Calibrate();
                }
                else
                {
                    timeEndError = Time.time + ErrorDuration;
                }
            }
        }

        protected virtual void Calibrate()
        {
            //Load  next step.
            FinchController.HapticPulse(NodeType.RightHand, FinchHapticPattern.LongClick);
            FinchController.HapticPulse(NodeType.LeftHand, FinchHapticPattern.LongClick);

            Internal.Calibration.ReplyManager.Calibration(Internal.FinchCore.Finch_CalibrationType.Hmd, Internal.FinchCore.Finch_CalibrationOptions.None);
            NextStep();
        }

        private void UpdateChirality()
        {
            if (FinchNodeManager.GetControllersCount() == 1 && (FinchController.LeftController.SwipeRight || FinchController.RightController.SwipeLeft))
            {
                FinchNodeManager.SwapNodes(NodeType.LeftHand, NodeType.RightHand);
            }

            bool differentRight = FinchController.RightController.IsConnected && FinchNodeManager.IsConnected(NodeType.LeftUpperArm);
            bool differentLeft = FinchController.LeftController.IsConnected && FinchNodeManager.IsConnected(NodeType.RightUpperArm);

            if (FinchNodeManager.GetUpperArmCount() == 1 && FinchNodeManager.GetControllersCount() == 1 && (differentRight || differentLeft))
            {
                FinchNodeManager.SwapNodes(NodeType.LeftUpperArm, NodeType.RightUpperArm);
            }
        }

        protected virtual void UpdateSprite()
        {
            bool bothController = (int)settings.Set % 10 == 2;
            bool errorConnect = (int)settings.Set % 10 != FinchNodeManager.GetControllersCount();
            bool wasLeftHint = CalibrateLeftHandWithArms.activeSelf || CalibrateLeftHandWithoutArms.activeSelf;

            bool haveUpperArms = (int)settings.Set / 10 > 0;
            bool leftConnected = !bothController && (errorConnect ? wasLeftHint : FinchController.LeftController.IsConnected);
            bool rightConnected =!bothController && (errorConnect ? !wasLeftHint : FinchController.RightController.IsConnected);

            bool IsSixDof = FinchNodeManager.GetUpperArmCount() > 0;

            if (haveUpperArms)
            {
                Notification.ID = bothController ? CalibrationPhraseId.CalibrateBothControllersWithUpperArms : CalibrationPhraseId.CalibrateOneControllerWithUpperArms;
            }
            else
            {
                Notification.ID = bothController ? CalibrationPhraseId.CalibrateBothControllersWithoutUpperArms : CalibrationPhraseId.CalibrateOneControllerWithoutUpperArms;
            }

            Header.Id = NotificationWord.Calibration;

            LeftSwapHint.SetActive(leftConnected);
            RightSwapHint.SetActive(rightConnected);

            if (GetHandAngle(CurrentAngleBorder) == HandHint.Base)
            {
                CurrentAngleBorder = angleBorderWide;
            }
            else
            {
                CurrentAngleBorder = CalibrationHorizontalError.AngleBorderSin;
            }

            CalibrateLeftHandWithArms.SetActive(haveUpperArms && leftConnected && !(GetHandAngle(CurrentAngleBorder) == HandHint.Base));
            CalibrateLeftHandWithoutArms.SetActive(!haveUpperArms && leftConnected);
            ReadyStanceLeft.SetActive(haveUpperArms && leftConnected && (GetHandAngle(CurrentAngleBorder) == HandHint.Base));

            CalibrateRigthHandWithoutArms.SetActive(!haveUpperArms && rightConnected);
            CalibrateRigthHandWithArms.SetActive(haveUpperArms && rightConnected && !(GetHandAngle(CurrentAngleBorder) == HandHint.Base));
            ReadyStanceRight.SetActive(haveUpperArms && rightConnected && (GetHandAngle(CurrentAngleBorder) == HandHint.Base));

            if (Time.time > timeEndError)
            {
                hint = HandHint.Base;
            }

            for (int i = 0; i < HorizontalError.Length; i++)
            {
                HorizontalError[i].SetActive((int)hint == i);
            }
        }

        private HandHint GetHandAngle()
        {
            return GetHandAngle(CalibrationHorizontalError.AngleBorderSin);
        }

        private HandHint GetHandAngle(float customAngle)
        {
            switch (Internal.Calibration.Calculations.GetNodesDirectionSign(customAngle, PitchControllerAngleBorderSin, RollAngleBorderSin))
            {
                case Internal.Calibration.ArmsDirection.Up:
                    return HandHint.LowerHand;

                case Internal.Calibration.ArmsDirection.Down:
                    return HandHint.UpperHand;

                case Internal.Calibration.ArmsDirection.LeanClockwise:
                case Internal.Calibration.ArmsDirection.LeanCounterClockwise:
                case Internal.Calibration.ArmsDirection.Different:
                    return HandHint.StraightHand;
            }

            return HandHint.Base;
        }

    }
}