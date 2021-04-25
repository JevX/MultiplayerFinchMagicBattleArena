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

public class PutOnFinchRingStep : TutorialStep
{
    public PutOnRingVisual Right;
    public PutOnRingVisual Left;

    bool oncePass = false;

    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        Init();
    }

    void Init()
    {
        if (oncePass)
            NextStep();

        Right.gameObject.SetActive(true);
        Right.Strated = true;
        Left.gameObject.SetActive(false);
        Right.Complete = false;
    }

    private void Update()
    {
        if (Right.Complete && Left.Complete)
        {
            NextStep();
            oncePass = true;
        }
        else if (Right.Complete && !Left.Complete)
        {
            Left.gameObject.SetActive(true);
            Left.Strated = true;
            Right.gameObject.SetActive(false);
        }
    }
}
