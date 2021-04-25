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
    public class ConnectionAnyStep : ConnectionBaseStep
    {
        [Header("Screens")]
        public GameObject WaitingConnection;
        public GameObject ConnectionFirstNode;
        public GameObject Launcher;
        public NotificationWords LauncherHeader;
        public NotificationCalibration LauncherHint;
        public NotificationCalibration FirstNodeHint;

        [Header("Launcher set")]
        public GameObject LaunchSet;
        public NotificationSet CurrentSetHint;
        public GameObject TouchpadHint;
        public GameObject ModesList;
        public NotificationSet[] AvailableModes = new NotificationSet[4];
        public ChangeState ButtonLaunch;
        public ChangeState ButtonChoose;

        [Header("Choose set")]
        public GameObject ChooseSet;
        public Transform ChooseSetButton;
        public NotificationSet[] Modes = new NotificationSet[4];
        public GameObject DisableBg;

        public ChangeState ControllerTutorial;
        public ChangeState ControllerArrow;
        public ChangeState UpperArmTutorial;
        public ChangeState UpperArmArrow;

        [Header("Silhouette")]
        public ChangeState[] Silhouettes = new ChangeState[4];

        [Header("Icons")]
        public DeviceIcons Icons;

        private const float endWaitingTime = 3f;
        private float timeLoadNextStep;
        private bool chooseSetScreenActive;
        private bool buttonLaunchActive;
        private bool oncePass;

        private float timePrePart;
        protected bool isPrePart;
        protected bool isDisconnect;

        private const float successDuration = 1.3f;
        private const float prePartDuration = 8.8f;


        [Header("Available sets")]
        public List<PlayableSet> SetsTurn = new List<PlayableSet>()
        {
            PlayableSet.OneThreeDof,
            PlayableSet.OneSixDof,
        };

        private PlayableSet currentSet;

        public override void Init(FinchCalibrationSettings calibrationSettings)
        {
            base.Init(calibrationSettings);
            buttonLaunchActive = true;
            chooseSetScreenActive = true;//false;

            if (FinchCalibration.TimeStampError)
            {
                timeStampsError = true;
            }
            else
            {
                timePrePart = Time.time + prePartDuration;
            }

            if (settings.Set != PlayableSet.Any || oncePass)
            {
                oncePass = true;
                NextStep();
            }
        }

        private void Update()
        {
            UpdateScanner(SetsTurn[SetsTurn.Count - 1]);
            UpdateScreen();
            UpdateLauncherButtons();

            UpdateTutorials(false);

            isPrePart = (Time.time < timePrePart); ;

            if ((timeStampsError && controllersConnected > 0 && upperArmConnected > 0))
            {
                return;
            }
            else if (timeStampsError && controllersConnected == 0 && upperArmConnected == 0)
            {
                timeStampsError = false;
                timePrePart = Time.time + prePartDuration;
                FinchCalibration.TimeStampError = false;
            }

            UpdateButtonController();
            UpdateIcons();
            UpdateChooseModeButtons();

        }

        private void UpdateScreen()
        {
            WaitingConnection.SetActive(Time.time < endWaitingTime);
            ConnectionFirstNode.SetActive(!WaitingConnection.activeSelf && controllersConnected == 0);
            Launcher.SetActive(!WaitingConnection.activeSelf && !ConnectionFirstNode.activeSelf);
            LaunchSet.SetActive(!chooseSetScreenActive);
            ChooseSet.SetActive(chooseSetScreenActive);
        }

        private void UpdateButtonController()
        {
            RingElement home = RingElement.HomeButton;
            bool pressedButton = (FinchController.GetPressDown(Chirality.Any, home) && FinchController.GetPressTime(Chirality.Any, home) < 2f) || Input.GetKeyDown(KeyCode.D);
            bool swipedUp =!loadNextStep && (FinchController.LeftController.SwipeTop || FinchController.RightController.SwipeTop || Input.GetKeyDown(KeyCode.W));
            bool swipedDown = !loadNextStep && (FinchController.LeftController.SwipeBottom || FinchController.RightController.SwipeBottom || Input.GetKeyDown(KeyCode.S));

            if (!Launcher.activeSelf)
            {
                currentSet = GetCurrentSet();
                return;
            }

            if (chooseSetScreenActive)
            {
                UpdateChooseSetScreen(FinchController.GetPressDown(Chirality.Any, home), swipedUp, swipedDown);
            }
            else
            {
                UpdateLaunchScreen(FinchController.GetPressDown(Chirality.Any, home) && FinchController.GetPressTime(Chirality.Any,home) < .5f, swipedUp, swipedDown);
            }
        }

        private void UpdateChooseSetScreen(bool pressedButton, bool swipedUp, bool swipedDown)
        {
            int currentId = SetsTurn.FindIndex(x => x == currentSet);

            if (swipedUp)
            {
                //Choose prev set.
                currentId--;
                if (currentId < 0)
                {
                    currentId = SetsTurn.Count - 1;
                }

                UpdateTutorials(true);

                currentSet = SetsTurn[currentId];
                Icons.ResetIcons((int)currentSet % 10, (int)currentSet / 10, controllersConnected, upperArmConnected, false);
            }

            if (swipedDown)
            {
                //Choose next set.
                currentId++;
                if (currentId > SetsTurn.Count - 1)
                {
                    currentId = 0;
                }

                UpdateTutorials(true);

                currentSet = SetsTurn[currentId];
                Icons.ResetIcons((int)currentSet % 10, (int)currentSet / 10, controllersConnected, upperArmConnected, false);
            }

            bool fullSet = (int)currentSet % 10 <= controllersConnected && (int)currentSet / 10 <= upperArmConnected;

            if (pressedButton && fullSet && !loadNextStep)
            {
                //Load next step
                loadNextStep = true;
                timeLoadNextStep = Time.time + 0.2f;
                FinchNodeManager.StopScan();
                settings.Set = currentSet;
                Internal.FinchNodeManager.NormalizeNodeCount((int)currentSet);
            }

            if (loadNextStep && Time.time > timeLoadNextStep)
            {
                loadNextStep = false;
                oncePass = true;
                settings.Set = currentSet;
                Internal.FinchNodeManager.NormalizeNodeCount((int)currentSet);
                isPrePart = false;
                CheckTimeStamps();
            }
        }

        private void UpdateLaunchScreen(bool pressedButton, bool swipedUp, bool swipedDown)
        {
            currentSet = GetCurrentSet();

            if (loadNextStep && Time.time > timeLoadNextStep)
            {
                loadNextStep = false;
                oncePass = true;
                settings.Set = currentSet;
                Internal.FinchNodeManager.NormalizeNodeCount((int)currentSet);
                isPrePart = false;
                CheckTimeStamps();
            }

            if (pressedButton && !loadNextStep)
            {
                if (buttonLaunchActive)
                {
                    FinchNodeManager.StopScan();
                    settings.Set = currentSet;
                    Internal.FinchNodeManager.NormalizeNodeCount((int)currentSet);
                    loadNextStep = true;
                    timeLoadNextStep = Time.time + 0.2f;
                }
                else
                {
                    chooseSetScreenActive = true;
                    Icons.ResetIcons((int)currentSet % 10, (int)currentSet / 10, controllersConnected, upperArmConnected, false);
                }
            }

            buttonLaunchActive |= swipedUp;
            buttonLaunchActive &= !swipedDown;
        }

        private PlayableSet GetCurrentSet()
        {
            for (int i = SetsTurn.Count - 1; i >= 0; i--)
            {
                if(controllersConnected >= (int)SetsTurn[i] % 10 && upperArmConnected >= (int)SetsTurn[i] / 10)
                {
                    return SetsTurn[i];
                }
            }

            return SetsTurn[0];
        }

        private void UpdateChooseModeButtons()
        {
            int currentId = SetsTurn.FindIndex(x => x == currentSet);
            if (currentId >= 0 && currentId < 2)
            {
                //Move and update indicator.
                Vector3 pos = ChooseSetButton.transform.localPosition;
                pos.y = Modes[currentId].transform.localPosition.y;
                ChooseSetButton.transform.localPosition = pos;
                DisableBg.SetActive((int)currentSet % 10 > controllersConnected || (int)currentSet / 10 > upperArmConnected);
            }

            for(int i = 0; i < 2; i++)
            {
                Modes[i].gameObject.SetActive(i < SetsTurn.Count);

                if (i < SetsTurn.Count)
                {
                    Modes[i].ID = SetsTurn[i];
                }
            }
        }

        private void UpdateLauncherButtons()
        {
            CurrentSetHint.ID = (PlayableSet)Mathf.Max((int)currentSet, 1);

            if (ButtonLaunch.FinishState != buttonLaunchActive || ButtonChoose.FinishState == buttonLaunchActive)
            {
                ButtonLaunch.FinishState = buttonLaunchActive;
                ButtonChoose.FinishState = !buttonLaunchActive;
                ButtonLaunch.ResetState(false);
                ButtonChoose.ResetState(false);
            }

            TouchpadHint.SetActive(buttonLaunchActive);
            ModesList.SetActive(!buttonLaunchActive);

            int idLastHint = 0;

            for (int i = 0; i < SetsTurn.Count; i++)
            {
                //Turn on available set hint.
                bool availableSet = (int)SetsTurn[i] % 10 <= controllersConnected && (int)SetsTurn[i] / 10 <= upperArmConnected;

                if (availableSet)
                {
                    AvailableModes[idLastHint].ID = SetsTurn[i];
                    AvailableModes[idLastHint].gameObject.SetActive(!buttonLaunchActive);

                    idLastHint++;
                }
            }

            for (int i = idLastHint; i < 2; i++)
            {
                AvailableModes[i].gameObject.SetActive(false);
            }
        }

        private void UpdateTutorials(bool force)
        {
            bool fullSet = (int)currentSet % 10 <= controllersConnected && (int)currentSet / 10 <= upperArmConnected;
            bool allconnected = controllersConnected + upperArmConnected == 4 || chooseSetScreenActive && fullSet;

            if (PrePart.activeSelf)
            {
                LauncherHint.ID = CalibrationPhraseId.Open;
                LauncherHeader.Id = NotificationWord.Ready;
            }
            else if (Error.activeSelf)
            {
                LauncherHint.ID = CalibrationPhraseId.Put;
                LauncherHeader.Id = NotificationWord.Warning;
            }
            else if (ConnectionFirstNode.activeSelf)
            {
                LauncherHint.ID = CalibrationPhraseId.ConnectNodes;
                LauncherHeader.Id = NotificationWord.Ready;
            }
            else
            {
                if (allconnected)
                {
                    LauncherHint.ID = CalibrationPhraseId.ReadyConnect;
                    LauncherHeader.Id = chooseSetScreenActive ? NotificationWord.ChooseMode : NotificationWord.Launch;
                }
                else if (controllersConnected < (int)currentSet % 10)
                {
                    LauncherHint.ID = CalibrationPhraseId.ConnectNodes;
                    LauncherHeader.Id = chooseSetScreenActive ? NotificationWord.ChooseMode : NotificationWord.Launch;
                }
                else
                {
                    LauncherHint.ID = CalibrationPhraseId.ConnectUpperArms;
                    LauncherHeader.Id = chooseSetScreenActive ? NotificationWord.ChooseMode : NotificationWord.Launch;
                }
            }

            bool silhouetteHint = fullSet || !chooseSetScreenActive;
            bool controllerHint = !fullSet && chooseSetScreenActive && controllersConnected < (int)currentSet % 10;
            bool upperArmHint = !fullSet && !controllerHint && chooseSetScreenActive && upperArmConnected < (int)currentSet / 10;

            bool activeOld = !silhouetteHint && !Silhouettes[0].AnimationPass ||
                             !controllerHint && !ControllerTutorial.AnimationPass ||
                             !upperArmHint && !UpperArmTutorial.AnimationPass;


            int controllersNeed = (int)currentSet % 10;
            int upperArmsNeed = Math.Min(controllersNeed, (int)currentSet / 10);

            PrePart.SetActive(isPrePart && !timeStampsError && !Error.activeSelf);

            UpdateState(Silhouettes[0], silhouetteHint && controllersNeed > 0, force, activeOld);
            UpdateState(Silhouettes[1], silhouetteHint && controllersNeed > 1, force, activeOld);
            UpdateState(Silhouettes[2], silhouetteHint && upperArmsNeed > 0, force, activeOld);
            UpdateState(Silhouettes[3], silhouetteHint && upperArmsNeed > 1, force, activeOld);

            UpdateState(ControllerTutorial, controllerHint, force, activeOld);
            UpdateState(ControllerArrow, controllerHint, force, activeOld);
            UpdateState(UpperArmTutorial, upperArmHint, force, activeOld);
            UpdateState(UpperArmArrow, upperArmHint, force, activeOld);

            CommonPart.SetActive(!timeStampsError && !isPrePart && !Error.activeSelf);
            Error.SetActive(timeStampsError && !isPrePart);
        }

        private void UpdateIcons()
        {
            if (!Launcher.activeSelf)
            {
                int set = (int)SetsTurn[SetsTurn.Count - 1];
                Icons.ResetIcons(set % 10, set / 10, controllersConnected, upperArmConnected, true);
            }

            Icons.Update(controllersConnected, upperArmConnected);
        }
    }
}
