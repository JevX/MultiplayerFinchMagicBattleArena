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
using System;

namespace Finch
{
    public class ConnectionsStep : ConnectionBaseStep
    {
        [Header("Icons")]
        public DeviceIcons Icons;
        public GameObject SuccessHint;
        public NotificationCalibration ConnectionHint;
        public NotificationWords LauncherHeader;

        public ChangeState ControllerConnection;
        public ChangeState ControllerConnectionArrow;
        public ChangeState UpperArmConnection;
        public ChangeState UpperArmConnectionArrow;
        public ChangeState SuccessPart;

        private const float successDuration = 1.3f;
        private const float prePartDuration = 8.8f;

        private float minTimeToStopScan;

        private float timeNextStep;
        private float timePrePart;

        protected bool isPrePart;
        protected bool isDisconnect;

        public override void Init(FinchCalibrationSettings calibrationSettings)
        {
            base.Init(calibrationSettings);
            controllersConnected = FinchNodeManager.GetControllersCount();
            upperArmConnected = FinchNodeManager.GetUpperArmCount();

            if (FinchCalibration.TimeStampError)
            {
                timeStampsError = true;
            }
            else
            {
                timePrePart = Time.time + prePartDuration;
            }

            Icons.ResetIcons((int)settings.Set % 10, (int)settings.Set / 10, controllersConnected, upperArmConnected, true);

            UpdateTutorial(true);

            loadNextStep = false;
            minTimeToStopScan = Time.time + 0.5f;

            if (controllersConnected >= (int)settings.Set % 10 && upperArmConnected >= (int)settings.Set / 10 && !timeStampsError)
            {
                CheckTimeStamps();
            }
        }

        private void Update()
        {
            LauncherHeader.Id = NotificationWord.Ready;

            UpdateScanner(settings.Set);
            UpdateTutorial(false);

            isPrePart = (Time.time < timePrePart) && !timeStampsError; ;

            if (timeStampsError && controllersConnected > 0 && upperArmConnected > 0)
            {
                return;
            }
            else if (timeStampsError && controllersConnected == 0 && upperArmConnected == 0)
            {
                timeStampsError = false;
                FinchCalibration.TimeStampError = false;
                timePrePart = Time.time + prePartDuration;
            }

            Icons.Update(controllersConnected, upperArmConnected);
            bool allConnected = (controllersConnected >= (int)settings.Set % 10 && upperArmConnected >= (int)settings.Set / 10);
            loadNextStep &= allConnected;

            if (allConnected && !loadNextStep && (timeNextStep - Time.time < 1f) && Time.time > minTimeToStopScan)
            {
                loadNextStep = true;
                FinchNodeManager.StopScan();
                Internal.FinchNodeManager.NormalizeNodeCount((int)settings.Set);
            }

            if (!allConnected || !SuccessPart.AnimationPass  || !ControllerConnection.AnimationPass || !UpperArmConnection.AnimationPass || Time.time < minTimeToStopScan)
            {
                timeNextStep = Time.time + successDuration;
            }

            if (Time.time > timeNextStep)
            {
                Internal.FinchNodeManager.NormalizeNodeCount((int)settings.Set);
                isPrePart = false;
                CheckTimeStamps();
            }

        }

        private void UpdateTutorial(bool force)
        {
            bool controller = controllersConnected < (int)settings.Set % 10;
            bool upperArm = !controller && upperArmConnected < (int)settings.Set / 10;
            bool success = !controller && !upperArm && !timeStampsError;

            bool activeOld = !controller && !ControllerConnection.AnimationPass ||
                         !upperArm && !UpperArmConnection.AnimationPass ||
                         !success && !SuccessPart.AnimationPass;

            if (PrePart.activeSelf)
            {
                ConnectionHint.ID = CalibrationPhraseId.Open;
                LauncherHeader.Id = NotificationWord.Ready;
            }
            else if (Error.activeSelf)
            {
                ConnectionHint.ID = CalibrationPhraseId.Put;
                LauncherHeader.Id = NotificationWord.Warning;
            }
            else if (CommonPart.activeSelf)
            {
                CalibrationPhraseId controlerPhrase = CalibrationPhraseId.ConnectNodes;
                ConnectionHint.ID = controller ? controlerPhrase : CalibrationPhraseId.ConnectUpperArms;
            }

            PrePart.SetActive(isPrePart && !timeStampsError && !Error.activeSelf);

            UpdateState(ControllerConnection, controller, force, activeOld);
            UpdateState(ControllerConnectionArrow, controller, force, activeOld);
            UpdateState(UpperArmConnection, upperArm, force, activeOld);
            UpdateState(UpperArmConnectionArrow, upperArm, force, activeOld);
            UpdateState(SuccessPart, success && !timeStampsError, force, activeOld);

            CommonPart.SetActive(!timeStampsError && !isPrePart && !Error.activeSelf);
            Error.SetActive(timeStampsError && !isPrePart);
            SuccessHint.SetActive(success && !activeOld && SuccessPart.AnimationPass && SuccessPart.FinishState && !timeStampsError);
        }
    }
}