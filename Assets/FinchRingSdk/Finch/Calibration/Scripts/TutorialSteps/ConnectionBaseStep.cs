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
    public abstract class ConnectionBaseStep : TutorialStep
    {
        protected bool loadNextStep = false;
        protected int controllersConnected = 0;
        protected int upperArmConnected = 0;

        private const float timeToStartScaner = 1.5f;
        private float rescanning;
        private float pausing;
        private bool scanPause;
        protected bool timeStampsError;

        public GameObject PrePart;
        public GameObject CommonPart;
        public GameObject Error;


        private void Start()
        {
            Internal.FinchCore.OnDisconnected += OnDisconnectNode;
        }

        private void OnDisconnectNode(NodeType node)
        {
            scanPause = false;
            rescanning = 0f;
            pausing = 0f;
        }

        protected void UpdateState(ChangeState obj, bool state, bool force, bool activeOld)
        {
            //Reset animation appear/dissappear without collision.
            if (activeOld && state && !force)
            {
                obj.FinishState = false;
                obj.ResetState(true);
                return;
            }

            if (obj.FinishState != state || force)
            {
                obj.FinishState = state;
                obj.ResetState(!obj.gameObject.activeInHierarchy || force);
            }
        }

        protected void UpdateScanner(PlayableSet maxSet)
        {
            //Remember first connected node or last active
            controllersConnected = Mathf.Min((int)maxSet % 10, FinchNodeManager.GetControllersCount());
            upperArmConnected = Mathf.Min((int)maxSet / 10, FinchNodeManager.GetUpperArmCount());

            if (Time.time > timeToStartScaner && !FinchNodeManager.IsScanning && !loadNextStep && !scanPause)
            {
                FinchNodeManager.StartScan();
            }

            if (!scanPause && FinchNodeManager.IsScanning && rescanning >= 6)
            {
                rescanning = 0f;
                FinchNodeManager.StopScan();
                scanPause = true;
            }
            else if (!scanPause && FinchNodeManager.IsScanning && rescanning < 6)
            {
                scanPause = false;
                rescanning += Time.deltaTime;
            }

            if (scanPause && pausing >= 4 && !FinchNodeManager.IsScanning)
            {
                pausing = 0;
                scanPause = false;
            }
            else if (scanPause && pausing < 4 && !FinchNodeManager.IsScanning)
            {
                pausing += Time.deltaTime;
            }
        }

        private static bool isNearTimestamps(int ts1, int ts2)
        {
            const int timeStampsAvailableDifference = 150;
            const int timeStampMaxBorder = 65536;
            const int timeStampAnotherMaxBorder = 59392;

            bool result = Mathf.Abs(ts1 - ts2) <= timeStampsAvailableDifference;
            result |= Mathf.Abs(ts1 - ts2 + timeStampMaxBorder) <= timeStampsAvailableDifference;
            result |= Mathf.Abs(ts1 - ts2 - timeStampMaxBorder) <= timeStampsAvailableDifference;
            result |= Mathf.Abs(ts1 - ts2 + timeStampAnotherMaxBorder) <= timeStampsAvailableDifference;
            result |= Mathf.Abs(ts1 - ts2 - timeStampAnotherMaxBorder) <= timeStampsAvailableDifference;

            return result;
        }

        protected void CheckTimeStamps()
        {
            if (FinchNodeManager.GetUpperArmCount() == 0)
            {
                timeStampsError = false;
                NextStep();
                return;
            }

            ulong rightHandTimeStamp = Internal.FinchNodeManager.GetNodeTimeStamp(NodeType.RightHand);
            ulong leftHandTimeStamp = Internal.FinchNodeManager.GetNodeTimeStamp(NodeType.LeftHand);
            ulong rightUpperArmTimeStamp = Internal.FinchNodeManager.GetNodeTimeStamp(NodeType.RightUpperArm);
            ulong leftUpperArmTimeStamp = Internal.FinchNodeManager.GetNodeTimeStamp(NodeType.LeftUpperArm);

            Debug.Log($"ConnectionBaseStep: rhTs = {rightHandTimeStamp}; ruaTs = {rightUpperArmTimeStamp}; lhTs = {leftHandTimeStamp}; luaTs = {leftUpperArmTimeStamp}");

            if (Internal.FinchNodeManager.CheckTimeStamps())
            {
                timeStampsError = false;
                NextStep();
            }
            else
            {
                timeStampsError = true;
            }
        }
    }
}
