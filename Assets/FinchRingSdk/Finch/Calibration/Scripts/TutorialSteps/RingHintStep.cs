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
using Finch;

public class RingHintStep : TutorialStep
{
    public GameObject ThreeDofHint;
    public GameObject SixDofHint;

    public NotificationCalibration Notification;
    public NotificationWords Header;

    private RecalibrationState recalibration = new RecalibrationState();

    private bool oncePass;

    public override void Init(FinchCalibrationSettings calibrationSettings)
    {
        base.Init(calibrationSettings);

        if (oncePass)
        {
            NextStep();
        }
        else
        {
            recalibration.Update(settings, true);
            Update();
        }
    }

    private void Update()
    {
        recalibration.Update(settings);

        ThreeDofHint.SetActive((int)settings.Set / 10 == 0);
        SixDofHint.SetActive((int)settings.Set / 10 > 0);

        Header.Id = NotificationWord.Calibration;

        if ((int)settings.Set / 10 == 0)
        {
            Notification.ID = CalibrationPhraseId.ControllerRecalibration;
        }
        else if ((int)settings.Set / 10 > 0)
        {
            Notification.ID = CalibrationPhraseId.UpperArmRecalibration;
        }

        if (recalibration.RecalibrationAvailable)
        {
            oncePass = true;
            NextStep();
        }
    }
}
