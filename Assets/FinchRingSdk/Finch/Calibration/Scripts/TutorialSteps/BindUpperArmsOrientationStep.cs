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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finch
{
    public class BindUpperArmsOrientationStep : TutorialStep
    {
        public NotificationCalibration Notification;
        public NotificationWords Header;

        private const float upperArmAngleBorder = 0.5f; //30 degrees

        private int prevUpperArmCount = 0;

        private bool initialState = false;
        private bool waitingCalibrationState = false;

        public override void Init(FinchCalibrationSettings settings)
        {
            bool useUpperArm = (int)settings.Set / 10 > 0;
            bool uncalibrateUpperArms = prevUpperArmCount != (int)settings.Set / 10;

            if (!settings.IsMomentalCalibration && useUpperArm && (settings.WasNodeReconnect || uncalibrateUpperArms))
            {
                base.Init(settings);
                waitingCalibrationState = false;

                initialState = true;
                Internal.Calibration.ReplyManager.Calibration(Internal.FinchCore.Finch_CalibrationType.Reset, Internal.FinchCore.Finch_CalibrationOptions.ResetReverting);
                Internal.Calibration.ReplyManager.CalibrationReply += DoAfterInitialState;
            }
            else
            {
                NextStep();
            }
        }

        private void Update()
        {
            if (!initialState)
            {
                HandleUpdate();
            }

            Notification.ID = CalibrationPhraseId.BindUpperArmOrientation;
            Header.Id = NotificationWord.Calibration;

        }

        private void DoAfterInitialState(object obj, EventArgs args)
        {
            initialState = false;
            //Save last UpperArm set.
            prevUpperArmCount = (int)settings.Set / 10;
            HandleUpdate();
        }

        private void HandleUpdate()
        {
            if (waitingCalibrationState)
            {
                return;
            }

            bool revertLeft;
            bool revertRight;

            if (TryBindOrientation(NodeType.RightUpperArm, out revertRight) && TryBindOrientation(NodeType.LeftUpperArm, out revertLeft))
            {
                if (revertLeft || revertRight)
                {
                    if (revertLeft)
                    {
                        Internal.FinchCore.Finch_RevertCalibration(Internal.FinchCore.Finch_Node.LeftUpperArm);
                    }

                    if (revertRight)
                    {
                        Internal.FinchCore.Finch_RevertCalibration(Internal.FinchCore.Finch_Node.RightUpperArm);
                    }

                    waitingCalibrationState = true;
                    Internal.Calibration.ReplyManager.Calibration(Internal.FinchCore.Finch_CalibrationType.None, Internal.FinchCore.Finch_CalibrationOptions.CalibrateReverting);
                    Internal.Calibration.ReplyManager.CalibrationReply += DoAfterWaitingCalibrationState;
                }
                else
                {
                    NextStep();
                }
            }
        }

        private void DoAfterWaitingCalibrationState(object obj, EventArgs args)
        {
            waitingCalibrationState = false;
            NextStep();
        }

        private bool TryBindOrientation(NodeType node, out bool shouldRevert)
        {
            var result = Internal.Calibration.Calculations.GetArmDirection((Internal.FinchCore.Finch_Node)node, upperArmAngleBorder);
            shouldRevert = (result == Internal.Calibration.ArmsDirection.Up);
            return (result != Internal.Calibration.ArmsDirection.Forward);
        }

    }
}
